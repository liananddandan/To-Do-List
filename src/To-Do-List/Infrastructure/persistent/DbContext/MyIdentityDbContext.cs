using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_Do_List.domain.Entities;

namespace To_Do_List.Infrastructure.persistent.DbContext;

public class MyIdentityDbContext(DbContextOptions<MyIdentityDbContext> options)
   : IdentityDbContext<MyUser, MyRole, long>(options);