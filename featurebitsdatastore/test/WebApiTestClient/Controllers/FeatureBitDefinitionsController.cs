/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using FeatureBitsData;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTestClient.Controllers
{
    [Route("api/[controller]")]
    public class FeatureBitDefinitionsController : Controller
    {
        private readonly FeatureBitsEfDbContext _context;

        public FeatureBitDefinitionsController(FeatureBitsEfDbContext context)
        {
            _context = context;
        }

        // GET api/featurebitdefinitions
        [HttpGet]
        public IEnumerable<FeatureBitDefinition> Get()
        {
            return _context.FeatureBitDefinitions;
        }
    }
}
