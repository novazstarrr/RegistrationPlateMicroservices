﻿global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Polly;
global using Catalog.API;
global using Catalog.API.Data;
global using Catalog.Domain;
global using Serilog;
global using System;
global using System.Collections.Generic;
global using System.Data.SqlClient;
global using System.IO;
global using System.Reflection;
global using System.Threading.Tasks;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using AutoMapper;
global using Catalog.API.DTOs.Requests;
global using Catalog.API.DTOs.Responses;
global using Catalog.API.Exceptions;
global using Catalog.API.Middleware;
global using Catalog.API.Validators;
