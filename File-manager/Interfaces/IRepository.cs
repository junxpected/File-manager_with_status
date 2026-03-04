using System;
using System.Collections.Generic;
using System.Text;

namespace File_manager.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Save(T item);
        IEnumerable<T> LoadAll ();

        void Commit();
    }
}
