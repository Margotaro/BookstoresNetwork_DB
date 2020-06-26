namespace BookStore
{
    interface IWorkerListener
    {
        void workerChanged(Worker worker);
        void workerCreated(Worker worker);
    }
}
