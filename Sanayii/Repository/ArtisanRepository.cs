﻿using Microsoft.EntityFrameworkCore;
using Sanayii.Core.Entities;
using Snai3y.Repository.Data;

namespace Sanayii.Repository
{
    public class ArtisanRepository : GenericRepository<Artisan>
    {
        public ArtisanRepository(SanayiiContext db) : base(db) { }

        public  List<Artisan> GetTopRatedArtisans()
        {
            return db.Artisans.OrderByDescending(a => a.Rating).ToList();
        }
    }
}
