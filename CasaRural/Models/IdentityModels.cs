using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CasaRural.Models
{
    // Para agregar datos de perfil del usuario, agregue más propiedades a su clase ApplicationUser. Visite https://go.microsoft.com/fwlink/?LinkID=317594 para obtener más información.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Llogater> Llogaters { get; set; }
        public DbSet<Reserva> Reservess { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reserva>()
                .HasRequired(r => r.Llogater)
                .WithMany(m => m.Reserves)
                .HasForeignKey(k => k.LlogaterId)
                .WillCascadeOnDelete(true);
        }
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = new DbEntityValidationResult(entityEntry, new List<DbValidationError>());

            if (entityEntry.Entity is Llogater &&
                (entityEntry.State == EntityState.Added ||
                entityEntry.State == EntityState.Modified))
            {
                Llogater llogater = entityEntry.Entity as Llogater;

                bool comprovacio_codi_postal = Regex.Match(llogater.CodiPostal + "", @"^([1-9]{2}|[0-9][1-9]|[1-9][0-9])[0-9]{3}$", RegexOptions.IgnoreCase).Success;
                bool comprovacio_nif = Regex.Match(llogater.NIF + "", @"^[0-9]{8}[A-Z]{1}$", RegexOptions.IgnoreCase).Success;
                bool comprovacio_nom = (llogater.NomLlogater.Length >= 20 && llogater.NomLlogater.Length <= 200);
                bool comprovacio_cognom = (llogater.CognomLlogater.Length >= 20 && llogater.CognomLlogater.Length <= 200);

                if (!comprovacio_codi_postal)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("CodiPostal", "El format no és el correcte"));
                }
                if (!comprovacio_nif)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("NIF", "El format del NIF no és el correcte"));
                }
                if (!comprovacio_nom)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("NomLlogater", "Mínim 20 caracters i màxim 200"));
                }
                if (!comprovacio_cognom)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("CognomLlogater", "Mínim 20 caracters i màxim 200"));
                }
                if (result.ValidationErrors.Count >= 0)
                {
                    return result;
                }
            }
            else if (entityEntry.Entity is Reserva &&
              (entityEntry.State == EntityState.Added ||
              entityEntry.State == EntityState.Modified))
            {
                Reserva reserva = entityEntry.Entity as Reserva;

                var fechaEntrada = reserva.DataEntrada;
                var fechaSalida = reserva.DataSortida;

                var comprovacio_data = new DateTime();
                comprovacio_data = comprovacio_data.AddDays(1);

                if (fechaEntrada > fechaSalida)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("DataEntrada", "La data d'entrada ha de ser superior a la data de sortida"));
                }
                if (fechaEntrada < comprovacio_data.Date)
                {
                    result.ValidationErrors.Add(
                        new System.Data.Entity.Validation.DbValidationError("DataEntrada", "La data d'entrada no pot ser del mateix día"));
                }
                if (result.ValidationErrors.Count >= 0)
                {
                    return result;
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }
}