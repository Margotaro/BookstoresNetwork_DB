namespace BookStore
{
    class WorkerChangeManager : IWorkerListener
    {

        public WorkerChangeManager(IDAO dao)
        {
            this.dao = dao;
        }

        IDAO dao;

        public void workerChanged(Worker worker)
        {
            dao.updateWorker(worker);
        }
        public void workerCreated(Worker worker)
        {
            dao.createWorker(worker);
        }
    }
}
