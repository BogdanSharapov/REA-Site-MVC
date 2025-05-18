using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace REASite.Areas.Identity.Data;
public class SiteUser : IdentityUser
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            UpdateUserName();
        }
    }

    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            UpdateUserName();
        }
    }

    public DateTime? DateOfBirth { get; set; }
    public GenderEnum Gender { get; set; } = GenderEnum.NotSpecified;


    public override string UserName
    {
        get => base.UserName;
        set => base.UserName = value;
    }

    private void UpdateUserName()
    {

        base.UserName = $"{FirstName}{LastName}".Trim();
    }
}
