using API.Models;

namespace API.Data
{
    public class InMemoryPatientRepository
    {
        public class PagedResult<T>
        {
            public List<T> Data { get; set; }
            public int TotalCount { get; set; }
        }


        private readonly List<Patient> _patients = new();
        private int _nextId = 1;

        public List<Patient> GetAll() => _patients;

        public Patient GetById(int id) => _patients.FirstOrDefault(p => p.Id == id);

        public void Add(Patient patient)
        {
            patient.Id = _nextId++;
            _patients.Add(patient);
        }

        public void Update(Patient patient)
        {
            var index = _patients.FindIndex(p => p.Id == patient.Id);
            if (index != -1)
            {
                _patients[index] = patient;
            }
        }

        public void Delete(int id)
        {
            var patient = GetById(id);
            if (patient != null)
            {
                _patients.Remove(patient);
            }
        }


        public PagedResult<Patient> GetPaged(int pageNumber, int pageSize, string? firstName, string? lastName, string? city, bool? active, string sortBy)
        {
            var query = _patients.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(p => p.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(p => p.LastName.Contains(lastName));
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.Contains(city));
            }

            if (active.HasValue)
            {
                if (active.Value) // If active is true, include active patients
                {
                    query = query.Where(p => p.Active);
                }
                else if (!active.Value)
                {
                    query = query.Where(p => !p.Active);
                }
                // No need for an else block, as we want all patients regardless of their active status
            }

            var totalCount = query.Count(); // Get the total count before pagination

            query = sortBy switch
            {
                "firstName" => query.OrderBy(p => p.FirstName),
                "lastName" => query.OrderBy(p => p.LastName),
                "city" => query.OrderBy(p => p.City),
                "active" => query.OrderBy(p => p.Active),
                _ => query
            };

            var pagedData = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<Patient> { Data = pagedData, TotalCount = totalCount };
        }

    }
}
