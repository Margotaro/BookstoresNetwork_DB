using System;
using System.Windows;
using System.Linq;

namespace BookStore
{
    public class WorkerChangeManager : IWorkerListener
    {

        public WorkerChangeManager(IDAO dao, IBookstoreWindow window)
        {
            this.dao = dao;
            this.window = window;
        }

        IDAO dao;
        IBookstoreWindow window;

        public void workerChanged(Worker worker)
        {
            dao.updateWorker(worker);
        }
        public void workerCreated(WorkerViewModel wvModel)
        {
            foreach (var v in wvModel.bookstores)
            {
                if (v.Address.ToString() == wvModel.bs_address)
                {
                    var shift = WorkingHours.Shifts.Single(p => p.shiftname == wvModel.t_shift);
                    dao.createWorker(wvModel.name, Convert.ToDouble(wvModel.salary), Position.manager, wvModel.login, wvModel.password, shift, new DateTime(wvModel.hiringdate.Year, wvModel.hiringdate.Month, wvModel.hiringdate.Day), dao.getBookstore(new Id<IBookstore>(v.BookstoreID)));
                    break;
                }
            }
            window.dataGridWorkerUpdate();
        }
        public void workerFired(Id<IWorker> id, DateTime firingDate)
        {
            dao.fireWorker(id, firingDate);//fireworker
            window.dataGridWorkerUpdate();
        }
    }
}
