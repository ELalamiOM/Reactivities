using Application.Profiles.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

// Contrôleur de gestion des profils utilisateurs
// Permet de gérer les photos de profil
public class ProfilesController : BaseApiController
{
    // POST: api/profiles
    // Upload une photo de profil - Route protégée (authentification requise)
    // La photo est stockée sur Cloudinary
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
    {
        return HandleResult(await Mediator.Send(new AddPhoto.Command { File = file }));
    }

    // DELETE: api/profiles/{id}
    // Supprime une photo de profil - Route protégée (authentification requise)
    // Supprime aussi la photo sur Cloudinary
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePhoto(string id)
    {
        return HandleResult(await Mediator.Send(new DeletePhoto.Command { Id = id }));
    }
}
