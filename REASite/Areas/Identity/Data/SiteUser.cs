using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace REASite.Areas.Identity.Data;

// Add profile data for application users by adding properties to the SiteUser class
public class SiteUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; } = string.Empty;

    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }
    public GenderEnum Gender { get; set; } = GenderEnum.NotSpecified;
}

