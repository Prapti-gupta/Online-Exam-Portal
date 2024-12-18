﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using onlineexamproject.Data;

#nullable disable

namespace onlineexamproject.Migrations
{
    [DbContext(typeof(onlineexamprojectContext))]
    [Migration("20240619073440_exam_mig")]
    partial class exam_mig
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("onlineexamproject.Models.Course", b =>
                {
                    b.Property<int>("Course_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Course_Id"));

                    b.Property<string>("Course_Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Course_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Course_Id");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("onlineexamproject.Models.Exam", b =>
                {
                    b.Property<int>("ExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExamId"));

                    b.Property<int>("Course_Id")
                        .HasColumnType("int");

                    b.Property<int>("Course_Id1")
                        .HasColumnType("int");

                    b.Property<int>("Course_Id2")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExamDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExamName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExamId");

                    b.HasIndex("Course_Id1");

                    b.HasIndex("Course_Id2");

                    b.ToTable("Exam");
                });

            modelBuilder.Entity("onlineexamproject.Models.Exam", b =>
                {
                    b.HasOne("onlineexamproject.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("Course_Id1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("onlineexamproject.Models.Course", "course")
                        .WithMany()
                        .HasForeignKey("Course_Id2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("course");
                });
#pragma warning restore 612, 618
        }
    }
}
