using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using web_front.Models;
using web_front.Models.DTO;

namespace web_front.Controllers
{
    public class ItemController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ItemController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        // GET: ItemController
        public async Task<ActionResult> Index()
        {
            using HttpClient client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost:7025/Item/GetItems"));
            var response = client.Send(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Item> itemData = JsonConvert.DeserializeObject<List<Item>>(responseBody);
                var items = new List<ItemDto>();
                foreach (var item in items)
                {
                    items.Add(item);
                }

                return View(itemData);
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            var itemDto = new ItemDto
            {
                Id = item.Item_Id,
                Name = item.Item_Name, // Map from Item to ItemDto
                Price = item.Price,
                Quantity = item.Quantity,
                Created_By = item.Created_By
            };

            using HttpClient client = _httpClientFactory.CreateClient();
            var url = new Uri("https://localhost:7025/Item/AddNewItem");
            var response = await client.PostAsJsonAsync<ItemDto>(url, itemDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Item was added successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add item";
            }
            return View();
        }
        // GET: ItemController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7025/Item/GetItemById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var itemString = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<Item>(itemString);
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Item item)
        {
            if (!ModelState.IsValid)
            {
                // Check and log validation errors
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Any())
                    {
                        var fieldName = entry.Key; // Name of the field
                        var fieldErrors = entry.Value.Errors.Select(e => e.ErrorMessage).ToList(); // List of error messages

                    }
                }
            }

            var itemDto = new UpdateItemDTO
            {
                Id = id,
                Name = item.Item_Name,
                Price = item.Price,
                Quantity = item.Quantity
            };

            using var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"https://localhost:7025/Item/UpdateItem/{id}", itemDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Item updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update item";
                return View(item);
            }
        }

        // GET: ItemController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7025/Item/GetItemById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var itemString = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<Item>(itemString);
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: ItemController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.DeleteAsync($"https://localhost:7025/Item/DeleteItem/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Item deleted successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete item";
                    return RedirectToAction("Delete", new { id });
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while attempting to delete the item.";
                return RedirectToAction("Delete", new { id });
            }
        }
    }
}
