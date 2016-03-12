namespace GiveLoveFirst.Boundary
{
    public interface IPermissions
    {
        bool IsInternal();

        bool IsSuper();
    }
}