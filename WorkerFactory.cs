using System;

namespace BookStore
{
    class WorkerFactory
    {

        public WorkerFactory(WorkerChangeManager changeManager)
        {
            this.changeManager = changeManager;
        }

        WorkerChangeManager changeManager;

        public IWorker makeModelWorker(string name, string position, double salary, string shiftname, string login, string password, string id = "DEFAULT")
        {
            Position p;
            bool shiftexists = false;
            WorkingHours wh = null;
            foreach (var shift in WorkingHours.Shifts)
            {
                if(shift.shiftname == shiftname)
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

            /// TODO: inser into db
            /// 
            return new Worker(changeManager, name, p, salary, wh, login, password, id);
        }
        public IWorker makeDBWorker(string name, string position, string salary, string shiftname, string login, string password, string id)
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

            /// TODO: inser into db
            /// 
            return new Worker(changeManager, name, p, Convert.ToDouble(salary), wh, login, password, id);
        }


    }
}
