using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Entities;

namespace To_Do_List.DbContext;

public class MyIdentityDbContext : IdentityDbContext<MyUser, MyRole, long>
{
   public MyIdentityDbContext(DbContextOptions<MyIdentityDbContext> options): base(options)
   {
      
   }
}