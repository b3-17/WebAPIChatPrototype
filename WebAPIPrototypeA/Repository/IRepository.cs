using System.Collections.Generic;

namespace Repository
{
	public interface IRepository<T>
	{
		void Save(T itemToSave);
		IEnumerable<T> All();
	}
}
