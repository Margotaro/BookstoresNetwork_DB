namespace BookStore
{
    public interface IWorkerListener
    {
        void workerChanged(Worker worker);
        void workerCreated(WorkerViewModel wvModel);
        void workerFired(Id<IWorker> id, DateTime firingDate);
    }
}
