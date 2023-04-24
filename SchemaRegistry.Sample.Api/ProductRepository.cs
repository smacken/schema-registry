public class ProductRepository
{
    private readonly List<Product> _products = new();

    public IEnumerable<Product> GetAll() => _products;

    public Product GetById(int id) => _products.Find(p => p.Id == id);

    public void Add(Product product) => _products.Add(product);

    public void Update(Product product)
    {
        int index = _products.FindIndex(p => p.Id == product.Id);
        _products[index] = product;
    }

    public void Delete(int id) => _products.Remove(GetById(id));
}