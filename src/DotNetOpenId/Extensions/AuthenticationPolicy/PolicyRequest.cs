﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace DotNetOpenId.Extensions.AuthenticationPolicy {
	public class PolicyRequest : IExtensionRequest {
		/// <summary>
		/// Instantiates a new <see cref="PolicyRequest"/>.
		/// </summary>
		public PolicyRequest() {
			PreferredPolicies = new List<string>(1);
		}

		/// <summary>
		/// Optional. If the End User has not actively authenticated to the OP within the number of seconds specified in a manner fitting the requested policies, the OP SHOULD authenticate the End User for this request.
		/// </summary>
		/// <remarks>
		/// The OP should realize that not adhering to the request for re-authentication most likely means that the End User will not be allowed access to the services provided by the RP. If this parameter is absent in the request, the OP should authenticate the user at its own discretion.
		/// </remarks>
		public TimeSpan? MaximumAuthenticationAge { get; set; }
		/// <summary>
		/// Zero or more authentication policy URIs that the OP SHOULD conform to when authenticating the user. If multiple policies are requested, the OP SHOULD satisfy as many as it can.
		/// </summary>
		/// <value>List of authentication policy URIs obtainable from the <see cref="AuthenticationPolicies"/> class or from a custom list.</value>
		/// <remarks>
		/// If no policies are requested, the RP may be interested in other information such as the authentication age.
		/// </remarks>
		public IList<string> PreferredPolicies { get; private set; }

		#region IExtensionRequest Members

		IDictionary<string, string> IExtensionRequest.Serialize(DotNetOpenId.RelyingParty.IAuthenticationRequest authenticationRequest) {
			var fields = new Dictionary<string, string>();

			if (MaximumAuthenticationAge.HasValue) {
				fields.Add(Constants.RequestParameters.MaxAuthAge, 
					MaximumAuthenticationAge.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture));
			}

			return fields;
		}

		bool IExtensionRequest.Deserialize(IDictionary<string, string> fields, DotNetOpenId.Provider.IRequest request) {
			Debug.Assert(fields != null);

			string maxAuthAge;
			MaximumAuthenticationAge = fields.TryGetValue(Constants.RequestParameters.MaxAuthAge, out maxAuthAge) ?
				TimeSpan.FromSeconds(double.Parse(maxAuthAge)) : (TimeSpan?)null;

			return true;
		}

		#endregion

		#region IExtension Members

		string IExtension.TypeUri {
			get { return Constants.TypeUri; }
		}

		#endregion
	}
}
