using System.ComponentModel.DataAnnotations;

namespace REASite.Models
{
    public enum OfferTypeEnum
    {
        [Display(Name = "Аренда")]
        Rent = 1,
        [Display(Name = "Продажа")]
        Sale = 2, 
        [Display(Name = "Посуточная аренда")]
        DailyRent = 3
    }
}
