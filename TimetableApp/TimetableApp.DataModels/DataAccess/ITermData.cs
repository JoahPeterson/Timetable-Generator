﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.DataAccess;
public interface ITermData
{
    Task CreateAsync(Term term);
    Task<Term?> GetByIdAsync(string id);
    Task<List<Term>> GetAsync();
    Task UpdateAsync(Term term);
}
