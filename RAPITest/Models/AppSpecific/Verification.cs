﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAPITest.Models.AppSpecific
{
	public interface Verification 
	{
		Result Verify(HttpResponse Response);
	}
}