namespace RideFlow.Core.Application.DTOs.Reports;

public class AttendanceSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
}