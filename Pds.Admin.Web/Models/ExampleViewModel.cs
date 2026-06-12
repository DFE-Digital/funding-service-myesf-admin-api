using Pds.Core.Web.Models;
using System;

namespace Pds.Admin.Web.Models
{
    /// <summary>
    /// The example view model.
    /// </summary>
    public class ExampleViewModel : BasePageViewModel
    {
        /// <inheritdoc/>
        public override string HeaderTitle => "Example";

        /// <inheritdoc/>
        public override bool ShowHeaderTitle => true;

        /// <inheritdoc/>
        public override string LogoutLink => "/account/logout";

        /// <summary>
        /// Gets or sets the hello string.
        /// </summary>
        public string Hello { get; set; }
    }
}