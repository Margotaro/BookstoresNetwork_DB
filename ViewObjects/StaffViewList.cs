using System.Collections.Generic;
using System.Collections.ObjectModel;
using BookStore;

public class StaffViewList
{
    public ObservableCollection<StaffListItem> list { get; set; }
    public StaffViewList(List<Worker> staff)
    {
        list = new ObservableCollection<StaffListItem>();
        foreach(var worker in staff)
        {
            list.Add(new StaffListItem(
                worker.id.ToString(),
                worker.name.ToString(),
                worker.hiring_date.ToString(),
                worker.dehiring_date.ToString(),
                worker.position.ToString(),
                worker.salary.ToString(),
                worker.hours.shiftname.ToString(),
                worker.working_place.address.ToString()
                ));
        }
    }
}
