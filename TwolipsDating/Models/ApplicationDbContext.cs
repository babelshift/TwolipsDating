﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ZipCode> ZipCodes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            SetupApplicationUserEntity(modelBuilder);
            SetupProfileEntity(modelBuilder);
            SetupGenderEntity(modelBuilder);
            SetupCountryEntity(modelBuilder);
            SetupCityEntity(modelBuilder);
            SetupUSStateEntity(modelBuilder);
            SetupZipCodeEntity(modelBuilder);
            SetupMessageStatusEntity(modelBuilder);
            SetupMessageEntity(modelBuilder);
        }

        private void SetupMessageEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.SenderApplicationUser)
                .WithMany(m => m.SentMessages)
                .HasForeignKey(m => m.SenderApplicationUserId);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.ReceiverApplicationUser)
                .WithMany(m => m.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverApplicationUserId);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.MessageStatus)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.MessageStatusId);

            modelBuilder.Entity<Message>()
                .Property(m => m.DateSent)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Subject)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Body)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Subject)
                .HasMaxLength(255);

            modelBuilder.Entity<Message>()
                .Property(m => m.Body)
                .HasMaxLength(2000);
        }

        private void SetupMessageStatusEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageStatus>()
                .HasKey(ms => ms.Id);

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Name)
                .IsRequired();

            modelBuilder.Entity<MessageStatus>()
                .Property(ms => ms.Name)
                .HasMaxLength(50);
        }

        private void SetupZipCodeEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZipCode>()
                .HasKey(z => z.ZipCodeId);

            modelBuilder.Entity<ZipCode>()
                .HasRequired(z => z.City)
                .WithMany(z => z.ZipCodes)
                .HasForeignKey(z => z.CityId);

            modelBuilder.Entity<ZipCode>()
                .Property(z => z.ZipCodeId)
                .HasMaxLength(5);
        }

        private void SetupUSStateEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<USState>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<USState>()
                .Property(u => u.Abbreviation)
                .IsRequired();

            modelBuilder.Entity<USState>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<USState>()
                .Property(u => u.Abbreviation)
                .HasMaxLength(2);

            modelBuilder.Entity<USState>()
                .Property(u => u.Name)
                .HasMaxLength(50);
        }

        private void SetupApplicationUserEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(a => a.Profile)
                .WithRequired(e => e.ApplicationUser);

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.IsActive)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.DateCreated)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.DateLastLogin)
                .IsRequired();
        }

        private void SetupCityEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<City>()
                .HasRequired(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.CountryId);

            modelBuilder.Entity<City>()
                .HasOptional(c => c.USState)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.USStateId);

            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .IsRequired();
        }

        private void SetupCountryEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Country>()
                .Property(c => c.Name)
                .IsRequired();
        }

        private void SetupGenderEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Name)
                .HasMaxLength(25);

            modelBuilder.Entity<Gender>()
                .Property(g => g.Name)
                .IsRequired();
        }

        private void SetupProfileEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .HasRequired(a => a.ApplicationUser)
                .WithOptional(a => a.Profile);

            modelBuilder.Entity<Profile>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.Gender)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.GenderId);

            modelBuilder.Entity<Profile>()
                .HasRequired(p => p.City)
                .WithMany(p => p.Profiles)
                .HasForeignKey(p => p.CityId);

            modelBuilder.Entity<Profile>()
                .Property(p => p.Birthday)
                .IsRequired();
        }
    }
}