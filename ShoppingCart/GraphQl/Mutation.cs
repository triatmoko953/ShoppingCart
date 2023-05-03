using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace ShoppingCart.GraphQl
{
    public class Mutation
    {
        public string Register([Service] ShoppingDbContext context, RegisterUser user)
        {
            // transaction
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    // tambah user
                    var u = new User
                    {
                        Username = user.Username,
                        FullName = user.FullName,
                        Password = BC.HashPassword(user.Password)
                    };

                    // ambil role member
                    var role = context.Roles.Where(o => o.Name == "Admin").FirstOrDefault();
                    // assign role ke user
                    if (role != null)
                    {
                        var ur = new UserRole();
                        ur.User = u;
                        ur.Role = role;

                        context.UserRoles.Add(ur);
                        // simpan dan commit
                        context.SaveChanges();
                        trans.Commit(); // commit
                        return "sukses";
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            }
            return "gagal";
        }

        public UserToken Login([Service] ShoppingDbContext context,
            [Service] IConfiguration configuration,
            UserLogin user)
        {
            // linq
            var usr = context.Users
                .Where(o => o.Username == user.Username).FirstOrDefault();
            if (usr != null)
            {
                if (BC.Verify(user.Password, usr.Password))
                {
                    // login sukses
                    // ambil role
                    //var userroles = _context.UserRoles.Where(o => o.UserId == usr.Id).ToList();                       
                    // joins
                    var roles = from ur in context.UserRoles
                                join r in context.Roles
                                on ur.RoleId equals r.Id
                                where ur.UserId == usr.Id
                                select r.Name;

                    var roleClaims = new Dictionary<string, object>();
                    foreach (var role in roles)
                    {
                        roleClaims.Add(ClaimTypes.Role, "" + role);
                    }


                    var secret = configuration.GetValue<string>("AppSettings:Secret");
                    var secretBytes = Encoding.ASCII.GetBytes(secret);

                    // token
                    var expired = DateTime.Now.AddDays(2); // 2 hari
                    var tokenHandler = new JwtSecurityTokenHandler();
                    // data
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        // payload
                        Subject = new System.Security.Claims.ClaimsIdentity(
                                new Claim[]
                                {
                                    new Claim(ClaimTypes.Name, user.Username)
                                }),
                        Claims = roleClaims, // claims - roles
                        Expires = expired,
                        SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(secretBytes),
                                SecurityAlgorithms.HmacSha256Signature
                            )
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var userToken = new UserToken
                    {
                        Token = tokenHandler.WriteToken(token), // token as string
                        ExpiredAt = expired.ToString(),
                        Message = ""
                    };
                    return userToken;
                }
            }
            return new UserToken { Message = "Invalid username or password" };
        }

        [Authorize(Roles = new[] { "Admin" })]
        public Product Create([Service] ShoppingDbContext context,Product p)
        {
            p.Deleted = false;
            context.Products.Add(p);
            context.SaveChanges();
            return p;
        }

        [Authorize(Roles = new[] { "Admin" })]
        public Product Update([Service] ShoppingDbContext context, int id, Product p)
        {
            var product = context.Products.FirstOrDefault(o => o.Id == id);
            if (product != null)
            {
                product.Name = p.Name;
                product.Price = p.Price;
                product.Stock = p.Stock;
            }
            context.Products.Update(product);
            context.SaveChanges();
            return product;
        }

        [Authorize(Roles = new[] { "Admin" })]
        public Product Delete([Service] ShoppingDbContext context, int id)
        {
            var product = context.Products.FirstOrDefault(o => o.Id == id);
            if (product != null)
            {
                product.Deleted=true;
            }
            context.Products.Update(product);
            context.SaveChanges();
            return product;
        }



    }
}
