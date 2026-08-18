using QuranSchool.Api.DTOs.SchoolInformation;
using QuranSchool.Api.Exceptions;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class SchoolInformationService(ISchoolInformationRepository repository)
    : ISchoolInformationService
{
    public async Task<SchoolInfoResponse?> GetAsync()
    {
        var info = await repository.GetWithDetailsAsync();
        if (info is null) return null;
        return MapToResponse(info);
    }

    public async Task<SchoolInfoResponse> CreateOrUpdateAsync(UpdateSchoolInfoRequest request)
    {
        var info = await repository.GetWithDetailsAsync();

        if (info is null)
        {
            info = new SchoolInformation
            {
                SchoolName = request.SchoolName,
                Description = request.Description,
                FoundedYear = request.FoundedYear,
                SchoolType = request.SchoolType,
                Address = request.Address,
                Phone = request.Phone,
                AdditionalPhone = request.AdditionalPhone,
                Email = request.Email,
                Whatsapp = request.Whatsapp,
                OfficialPage = request.OfficialPage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await repository.AddAsync(info);
            await repository.SaveChangesAsync();

            var days = new[] { "السبت", "الأحد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة" };
            foreach (var day in days)
            {
                var wh = new SchoolWorkingHours
                {
                    SchoolInformationId = info.Id,
                    DayOfWeek = day,
                    IsOpen = day != "الجمعة",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await repository.AddWorkingHoursAsync(wh);
            }
            await repository.SaveChangesAsync();
        }
        else
        {
            info.SchoolName = request.SchoolName;
            info.Description = request.Description;
            info.FoundedYear = request.FoundedYear;
            info.SchoolType = request.SchoolType;
            info.Address = request.Address;
            info.Phone = request.Phone;
            info.AdditionalPhone = request.AdditionalPhone;
            info.Email = request.Email;
            info.Whatsapp = request.Whatsapp;
            info.OfficialPage = request.OfficialPage;
            info.UpdatedAt = DateTime.UtcNow;
            repository.Update(info);
            await repository.SaveChangesAsync();
        }

        var result = await repository.GetWithDetailsAsync();
        return MapToResponse(result!);
    }

    public async Task<SchoolWorkingHoursDto> UpdateWorkingHoursAsync(UpdateWorkingHoursRequest request)
    {
        var info = await repository.GetWithDetailsAsync();
        if (info is null)
            throw new BadRequestException("يجب إنشاء معلومات المدرسة أولاً");

        var wh = info.WorkingHours.FirstOrDefault(w => w.DayOfWeek == request.DayOfWeek);
        if (wh is null)
            throw new NotFoundException("اليوم غير موجود");

        wh.IsOpen = request.IsOpen;
        wh.UpdatedAt = DateTime.UtcNow;

        foreach (var period in wh.Periods.ToList())
        {
            await repository.DeletePeriodAsync(period);
        }

        foreach (var p in request.Periods)
        {
            var period = new SchoolWorkingPeriod
            {
                WorkingHoursId = wh.Id,
                OpeningTime = p.OpeningTime,
                ClosingTime = p.ClosingTime
            };
            wh.Periods.Add(period);
        }

        repository.UpdateEntity(wh);
        await repository.SaveChangesAsync();

        var updated = await repository.GetWithDetailsAsync();
        var updatedWh = updated!.WorkingHours.First(w => w.Id == wh.Id);

        return new SchoolWorkingHoursDto
        {
            Id = updatedWh.Id,
            DayOfWeek = updatedWh.DayOfWeek,
            IsOpen = updatedWh.IsOpen,
            Periods = updatedWh.Periods.Select(p => new SchoolWorkingPeriodDto
            {
                Id = p.Id,
                OpeningTime = p.OpeningTime,
                ClosingTime = p.ClosingTime
            }).ToList()
        };
    }

    public async Task DeleteWorkingPeriodAsync(int periodId)
    {
        var period = await repository.GetPeriodByIdAsync(periodId);
        if (period is null) throw new NotFoundException("الفترة غير موجودة");
        await repository.DeletePeriodAsync(period);
    }

    public async Task<SchoolRuleDto> CreateRuleAsync(CreateSchoolRuleRequest request)
    {
        var info = await repository.GetWithDetailsAsync();
        if (info is null)
            throw new BadRequestException("يجب إنشاء معلومات المدرسة أولاً");

        var maxOrder = info.Rules.Any() ? info.Rules.Max(r => r.DisplayOrder) + 1 : 0;

        var rule = new SchoolRule
        {
            SchoolInformationId = info.Id,
            Title = request.Title,
            DisplayOrder = maxOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddRuleAsync(rule);
        await repository.SaveChangesAsync();

        return new SchoolRuleDto
        {
            Id = rule.Id,
            Title = rule.Title,
            DisplayOrder = rule.DisplayOrder,
            IsActive = rule.IsActive
        };
    }

    public async Task<SchoolRuleDto> UpdateRuleAsync(int id, UpdateSchoolRuleRequest request)
    {
        var rule = await repository.GetRuleByIdAsync(id);
        if (rule is null) throw new NotFoundException("القانون غير موجود");

        rule.Title = request.Title;
        rule.DisplayOrder = request.DisplayOrder;
        rule.IsActive = request.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        repository.UpdateEntity(rule);
        await repository.SaveChangesAsync();

        return new SchoolRuleDto
        {
            Id = rule.Id,
            Title = rule.Title,
            DisplayOrder = rule.DisplayOrder,
            IsActive = rule.IsActive
        };
    }

    public async Task DeleteRuleAsync(int id)
    {
        var rule = await repository.GetRuleByIdAsync(id);
        if (rule is null) throw new NotFoundException("القانون غير موجود");
        await repository.DeleteRuleAsync(rule);
    }

    public async Task ReorderRulesAsync(ReorderRulesRequest request)
    {
        var info = await repository.GetWithDetailsAsync();
        if (info is null) throw new NotFoundException("معلومات المدرسة غير موجودة");

        for (int i = 0; i < request.RuleIds.Count; i++)
        {
            var rule = info.Rules.FirstOrDefault(r => r.Id == request.RuleIds[i]);
            if (rule is not null)
            {
                rule.DisplayOrder = i;
                rule.UpdatedAt = DateTime.UtcNow;
            }
        }

        await repository.SaveChangesAsync();
    }

    private static SchoolInfoResponse MapToResponse(SchoolInformation info)
    {
        return new SchoolInfoResponse
        {
            Id = info.Id,
            SchoolName = info.SchoolName,
            Description = info.Description,
            FoundedYear = info.FoundedYear,
            SchoolType = info.SchoolType,
            Address = info.Address,
            Phone = info.Phone,
            AdditionalPhone = info.AdditionalPhone,
            Email = info.Email,
            Whatsapp = info.Whatsapp,
            OfficialPage = info.OfficialPage,
            CreatedAt = info.CreatedAt,
            UpdatedAt = info.UpdatedAt,
            WorkingHours = info.WorkingHours.Select(w => new SchoolWorkingHoursDto
            {
                Id = w.Id,
                DayOfWeek = w.DayOfWeek,
                IsOpen = w.IsOpen,
                Periods = w.Periods.Select(p => new SchoolWorkingPeriodDto
                {
                    Id = p.Id,
                    OpeningTime = p.OpeningTime,
                    ClosingTime = p.ClosingTime
                }).ToList()
            }).ToList(),
            Rules = info.Rules.OrderBy(r => r.DisplayOrder).Select(r => new SchoolRuleDto
            {
                Id = r.Id,
                Title = r.Title,
                DisplayOrder = r.DisplayOrder,
                IsActive = r.IsActive
            }).ToList()
        };
    }
}
