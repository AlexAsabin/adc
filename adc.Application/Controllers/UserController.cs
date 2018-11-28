using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using adc.Application.Core;
using adc.Core.Constants;
using adc.Core.PredicateCombinators;
using adc.Dal.Implementation;
using adc.Dal.Interfaces;
using adc.Dtos.Base;
using adc.Dtos.Core;
using adc.Dtos.Security;
using adc.Entities.Security;
using adc.Services.Interfaces;
using adc.Services.Interfaces.Core;

namespace adc.Application.Controllers {
    [Authorize(Roles = Roles.Admin)]
    [Route("api/[controller]")]
    public class UserController : BaseController {

        #region Variables

        private readonly IUserService _service;

        #endregion

        #region Constructor

        public UserController(IServiceProvider serviceProvider) : base(serviceProvider) {
            _service = ServiceProvider.GetRequiredService<IUserService>();
        }

        #endregion

        #region Api

        [HttpGet("{id}")]
        public IActionResult Get(int id) {
            var includes = _service.CreateIncludes(x => x.UserRoles.Select(y => y.Role));
            return Ok(_service.GetSingle(x => x.Id == id, includes));
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult List([FromBody]SearchModel model) {
            return Ok(new UsersResult {
                Items = _service.GetList(
                    predicate: SearchPredicate(model),
                    orderBys: String.Equals(model.Direction, "asc", StringComparison.InvariantCultureIgnoreCase) ? AscendingOrderBys(model.Sort) : DescendingOrderBys(model.Sort),
                    includes: _service.CreateIncludes(x => x.UserRoles.Select(y => y.Role)),
                    skip: model.Skip,
                    take: model.Limit),
                Total = _service.Count(predicate: SearchPredicate(model)),
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody]UserModel model) {
            try {
                return Ok(_service.Create(model));
            } catch (Exception e) {
                var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger(this.GetType().FullName);
                logger?.LogError(e, $"{Environment.NewLine}{GetMessage()}");
                return StatusCode((int)HttpStatusCode.ExpectationFailed, e.Message);
            }

        }

        [HttpPut]
        public IActionResult Update([FromBody]UserModel model) {
            try {
                return Ok(_service.Update(model));
            } catch (Exception e) {
                var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger(this.GetType().FullName);
                logger?.LogError(e, $"{Environment.NewLine}{GetMessage()}");
                return StatusCode((int)HttpStatusCode.ExpectationFailed, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            var includes = _service.CreateIncludes(x => x.UserRoles.Select(y => y.Role));
            var model = _service.GetSingle(x => x.Id == id, includes);
            if (model == null) {
                return NotFound();
            }

            // Prevent disable last admin
            if (model.IsAdmin && !model.InActive) {
                var countAdmins = _service.Count(
                    x => x.Id != model.Id && !x.InActive && x.UserRoles.Any(y => y.Role != null && y.Role.Name == Roles.Admin),
                    includes
                );
                if (countAdmins == 0) {
                    return StatusCode((int)HttpStatusCode.ExpectationFailed, "Unable deactivate last administrator.");
                }
            }
            _service.Update(x => x.Id == id, action: x => x.InActive = !x.InActive);
            return NoContent();
        }

        #endregion

        #region Helpers

        private Expression<Func<User, bool>> SearchPredicate(SearchModel model) {
            Expression<Func<User, bool>> predicate = u => true;

            if (model == null) {
                return predicate;
            }

            if (!string.IsNullOrEmpty(model.Query)) {
                predicate = predicate.And(u => (u.FirstName.Contains(model.Query)
                                               || u.LastName.Contains(model.Query))
                                               || u.Email.Contains(model.Query));
            }

            return predicate;
        }

        private List<IOrderBy<User>> DescendingOrderBys(string field) {
            if (string.Equals(field, "FirstName", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderByDescending<User, string>(x => x.FirstName));
            }

            if (string.Equals(field, "LastName", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderByDescending<User, string>(x => x.LastName));
            }

            if (string.Equals(field, "IsActive", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderByDescending<User, bool>(x => x.InActive));
            }

            return _service.CreateOrderBys(new OrderByDescending<User, string>(x => x.FirstName));
        }

        private List<IOrderBy<User>> AscendingOrderBys(string field) {
            if (string.Equals(field, "FirstName", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderBy<User, string>(x => x.FirstName));
            }

            if (string.Equals(field, "LastName", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderBy<User, string>(x => x.LastName));
            }

            if (string.Equals(field, "IsActive", StringComparison.InvariantCultureIgnoreCase)) {
                return _service.CreateOrderBys(new OrderBy<User, bool>(x => x.InActive));
            }

            return _service.CreateOrderBys(new OrderBy<User, string>(x => x.FirstName));
        }

        #endregion

    }
}
