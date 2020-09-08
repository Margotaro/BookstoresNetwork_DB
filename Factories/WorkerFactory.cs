using System;
using System.Windows.Documents;

namespace BookStore
{
    class WorkerFactory
    {

        public WorkerFactory(WorkerChangeManager changeManager, DAO dao)
        {
            this.changeManager = changeManager;
            this.dao = dao;
        }

        WorkerChangeManager changeManager;
        DAO dao;
        public IWorker makeWorker(string name, string position, string salary, string shiftname, string bookstoreid, string login, string password, string hiringdate_year, string hiringdate_month, string hiringdate_day, string id = "", string dehiringdate_year = "0", string dehiringdate_month = "0", string dehiringdate_day="0")
        {
            Position p;
            bool shiftexists = false;
            WorkingHours wh = null;
            foreach (var shift in WorkingHours.Shifts)
            {
                if (shift.shiftname == shiftname)
                {
                    wh = shift;
                    shiftexists = true;
                    break;
                }
            }
            if (!Enum.TryParse<Position>(position, out p) || !shiftexists)
            {
                throw new Exception("make worker has incorrect input position or shift");
            }

            DateTime hiringdate = new DateTime(Convert.ToInt32(hiringdate_year), Convert.ToInt32(hiringdate_month), Convert.ToInt32(hiringdate_day));
            DateTime dehiringdate = new DateTime(Convert.ToInt32(dehiringdate_year), Convert.ToInt32(dehiringdate_month), Convert.ToInt32(dehiringdate_day));
            Bookstore workplace = dao.getBookstore(new Id<IBookstore>(bookstoreid));

            return new Worker(changeManager, name, p, Convert.ToDouble(salary), wh, login, password, id, hiringdate, dehiringdate, workplace);
        }


    }
}
