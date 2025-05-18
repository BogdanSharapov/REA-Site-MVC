using System.ComponentModel.DataAnnotations;

namespace REASite.Models
{
    public enum ComfortEnum
    {
        [Display(Name = "Wi-Fi")]
        WiFi = 1,
        [Display(Name = "Стиральная машина")]
        WashingMachine,
        [Display(Name = "Кондиционер")]
        AirConditioner,
        [Display(Name = "Балкон")]
        Balcony,
        [Display(Name = "Парковка")]
        Parking,
        [Display(Name = "Лифт")]
        Elevator,
        [Display(Name = "Посудомоечная машина")]
        Dishwasher,
        [Display(Name = "Холодильник")]
        Refrigerator,
        [Display(Name = "Микроволновка")]
        Microwave,
        [Display(Name = "Мебель")]
        Furniture,
        [Display(Name = "Телевизор")]
        TV,
        [Display(Name = "Ванна")]
        Bathtub,
        [Display(Name = "Душевая кабина")]
        Shower,
        [Display(Name = "Бассейн")]
        SwimmingPool,
        [Display(Name = "Спортзал")]
        Gym,
        [Display(Name = "Детская площадка")]
        Playground,
        [Display(Name = "Охрана")]
        Security,
        [Display(Name = "Консьерж")]
        Concierge
    }
}