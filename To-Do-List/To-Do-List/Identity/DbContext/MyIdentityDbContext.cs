using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Identity.Entities;

namespace To_Do_List.Identity.DbContext;

public class MyIdentityDbContext : IdentityDbContext<MyUser, MyRole, long>
{
   public MyIdentityDbContext(DbContextOptions<MyIdentityDbContext> options): base(options)
   {
      
   }
}