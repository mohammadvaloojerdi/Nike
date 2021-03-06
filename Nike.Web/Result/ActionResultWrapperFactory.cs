﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nike.Web.Result.Wrappers;

namespace Nike.Web.Result
{
    public class ActionResultWrapperFactory : IActionResultWrapperFactory
    {
        public IActionResultWrapper CreateFor(ResultExecutingContext actionResult)
        {
            if (actionResult is null) 
                throw new ArgumentException();

            return actionResult.Result switch
            {
                ObjectResult _ => new ObjectActionResultWrapper(),
                JsonResult _ => new JsonActionResultWrapper(),
                EmptyResult _ => new EmptyActionResultWrapper(),
                _ => new NullAbpActionResultWrapper()
            };
        }
    }
}