using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using EVisaTicketSystem.Core.DTOs;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile file, Guid ticketId);
        Task<bool> DeletePhotoAsync(string filePath);
    }
}
