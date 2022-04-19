using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignmentfive.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
    }

    public class UserMetaData
    {
        [Remote("IsUserNameAvailable", "Accounts", HttpMethod = "POST", ErrorMessage = "UserName already in use.")]
        public string UserName { get; set; }
    }
}