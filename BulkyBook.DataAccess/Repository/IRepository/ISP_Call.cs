using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        T Single<T>(string produreName, DynamicParameters parameters = null);

        void Execute(string produreName, DynamicParameters parameters = null);

        T OneRecord<T>(string produreName, DynamicParameters parameters = null);

        IEnumerable<T> List<T>(string produreName, DynamicParameters parameters = null);

        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string produreName, DynamicParameters parameters = null);
    }
}
