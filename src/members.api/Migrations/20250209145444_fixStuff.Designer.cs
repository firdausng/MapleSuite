﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using members.api.infra.data;

#nullable disable

namespace members.api.Migrations
{
    [DbContext(typeof(MemberContext))]
    [Migration("20250209145444_fixStuff")]
    partial class fixStuff
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("members.api.domains.entities.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.ComplexProperty<Dictionary<string, object>>("Biographical", "members.api.domains.entities.Member.Biographical#Biographical", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<DateTime>("DateOfBirth")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Contact", "members.api.domains.entities.Member.Contact#Contact", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("PhoneNumber")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("WorkEmail")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Demographic", "members.api.domains.entities.Member.Demographic#Demographic", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Enthic")
                                .HasColumnType("text");

                            b1.Property<string>("Gender")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("MaritalStatus")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Religion")
                                .HasColumnType("text");
                        });

                    b.HasKey("Id");

                    b.ToTable("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
