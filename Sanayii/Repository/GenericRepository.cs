﻿using Snai3y.Repository.Data;

namespace Sanayii.Repository
{
    public class GenericRepository<TEntity> : IEntity<TEntity> where TEntity : class
    {
        public SanayiiContext db;

        public GenericRepository(SanayiiContext db)
        { 
            this.db = db;
        }

        public List<TEntity> getAll()
        {
            List<TEntity> allEntity = db.Set<TEntity>().ToList();
            return allEntity;
        }

        public TEntity getById<T>(T id)
        {
            TEntity entity = db.Set<TEntity>().Find(id);
            return entity;
        }

        public void add(TEntity entity)
        {
            db.Set<TEntity>().Add(entity);
        }

        public void edit(TEntity entity)
        {
            db.Update(entity);
        }

        public void delete<T>(T id)
        {
            TEntity entity = getById(id);
            db.Set<TEntity>().Remove(entity);
        }

    }
}
