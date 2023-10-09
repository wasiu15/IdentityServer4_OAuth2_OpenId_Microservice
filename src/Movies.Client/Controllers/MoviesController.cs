﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.ApiServices;
using Movies.Client.Models;
using System.Diagnostics;

namespace Movies.Client.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMovieApiServices _movieApiService;

        public MoviesController(IMovieApiServices movieApiService)
        {
            _movieApiService = movieApiService;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> OnlyAdmin()
        {
            var userInfo = await _movieApiService.GetUserInfo();
            return View(userInfo);
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            await LogTokenAndClaims();
            return View(await _movieApiService.GetMovies());
        }

        public async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _movieApiService.GetMovie(id));

            //return View();

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var movie = await _movieApiService.Movie
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (movie == null)
            //{
            //    return NotFound();
            //}

            //return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Rating,ImageUrl,ReleaseDate,Owner")] Movie movie)
        {
            return View();
            //if (ModelState.IsValid)
            //{
            //    _movieApiService.Add(movie);
            //    await _movieApiService.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _movieApiService.GetMovie(id));
            
            //return View(); 


            //if (id == null || _movieApiService.Movie == null)
            //{
            //    return NotFound();
            //}

            //var movie = await _movieApiService.Movie.FindAsync(id);
            //if (movie == null)
            //{
            //    return NotFound();
            //}
            //return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Rating,ImageUrl,ReleaseDate,Owner")] Movie movie)
        {
            return View();

            //if (id != movie.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _movieApiService.Update(movie);
            //        await _movieApiService.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!MovieExists(movie.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _movieApiService.GetMovie(id));

            //return View();

            //if (id == null || _movieApiService.Movie == null)
            //{
            //    return NotFound();
            //}

            //var movie = await _movieApiService.Movie
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (movie == null)
            //{
            //    return NotFound();
            //}

            //return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return View();

            //if (_movieApiService.Movie == null)
            //{
            //    return Problem("Entity set 'MoviesClientContext.Movie'  is null.");
            //}
            //var movie = await _movieApiService.Movie.FindAsync(id);
            //if (movie != null)
            //{
            //    _movieApiService.Movie.Remove(movie);
            //}

            //await _movieApiService.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return true;
            //return (_movieApiService.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
