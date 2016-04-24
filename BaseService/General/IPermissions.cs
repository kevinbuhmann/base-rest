namespace BaseService.General
{
    public interface IPermissions
    {
        bool IsInternal();

        bool IsSuper();
    }
}