﻿using BaseDomain.General;

namespace BaseService.General
{
    public interface IService<TDmn, TDto, TConverter, TPermissions>
        where TDmn : IDomain
        where TDto : IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TPermissions : IPermissions
    {
        TDto[] GetAll(string[] includes = null);

        TDto Get(int id, string[] includes = null);

        TDto Create(TDto dto);

        bool Update(int id, TDto dto);

        bool Delete(int id);

        void SetPermissions(TPermissions permissions);
    }
}