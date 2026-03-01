using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InvestmentCalculator.Models;

public class Calculation
{
    public int Id { get; set; }
    public double InitialAmount { get; set; }
    public double MonthlyContribution { get; set; }
    public double InterestRate { get; set; }
    public int Years { get; set; }
    public double FutureValue { get; set; }
    public double TotalInvested { get; set; }
    public double TotalInterest { get; set; }
    public DateTime CalculationDate { get; set; } = DateTime.Now;
}
