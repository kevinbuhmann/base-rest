namespace BaseRest.Boundary
{
    public interface IPermissions
    {
        bool IsInternal();

        bool IsSuper();

        bool IsSuperOrInternal();
    }
}