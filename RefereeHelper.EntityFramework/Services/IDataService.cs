using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework.Services
{
    public interface IDataService<T>
    {
        /// <summary>
        /// Получает все записи сущности T
        /// </summary>
        /// <returns>IEnumerable<typeparamref name="T"/>.</returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Получает все записи, соотвествующие принимаемому предикату
        /// </summary>
        /// <param name="predicate">, на основании которого будут выбираться экземпляры сущности</param>
        /// <returns><paramref name="IEnumerableT"/>, которые соответствуют <paramref name="predicate"/></returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Получает объект, с получаемым номером
        /// </summary>
        /// <param name="id"> - идентитфикационный номер экземпляра сущности</param>
        /// <returns>Объект типа <typeparamref name="T"></typeparamref>, который соотвествует принимаемому номеру</returns>
        Task<T> Get(int id);

        /// <summary>
        /// Добавляет запись в сущность.
        /// </summary>
        /// <param name="entity"> - экземпляр сущности, который необходимо добавить</param>
        /// <returns>Добавленный объект <paramref name="entity"/>.</returns>
        Task<T> Create (T entity);

        /// <summary>
        /// Обновляет запись в базе данных
        /// </summary>
        /// <param name="id"> - номер записи, которую будет заменяться.</param>
        /// <param name="entity"> - экземпляр сущности, на который она будет заменяться.</param>
        /// <returns>Возвращает, добавляенный экземпляр сущности <paramref name="entity"/></returns>
        Task<T> Update (int id, T entity);

        /// <summary>
        /// Удаляет запись в базе данных с определённым номером
        /// </summary>
        /// <param name="id"> - номер записи, которая будет удалена</param>
        /// <returns>Экземпляр сущности, которая была удалена</returns>
        Task<bool> Delete (int id);
    }
}
