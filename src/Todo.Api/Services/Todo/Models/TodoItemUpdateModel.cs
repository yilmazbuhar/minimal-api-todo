namespace Todo.Api
{
    public class TodoItemUpdateModel : TodoItemSaveModel
    {
        public bool Done { get; set; }
    }
}
