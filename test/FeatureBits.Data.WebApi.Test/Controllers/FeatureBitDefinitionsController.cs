// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBits.Data.WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    public class IFeatureBitDefinitionsController : Controller
    {
        private readonly FeatureBitsEfDbContext _context;

        public IFeatureBitDefinitionsController(FeatureBitsEfDbContext context)
        {
            _context = context;
        }

        // GET api/featurebitdefinitions
        [HttpGet]
        public IEnumerable<IFeatureBitDefinition> Get()
        {
            return _context.IFeatureBitDefinitions;
        }
    }
}
