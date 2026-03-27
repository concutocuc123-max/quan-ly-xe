using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleManagement
{
    
    public interface IInsurable
    {
        double CalculateInsuranceFee();
    }

    
    // 1. LỚP TRỪU TƯỢNG (ABSTRACTION & ENCAPSULATION)
    
    public abstract class Vehicle : IInsurable
    {
        // Encapsulation
        private int _year;
        private double _price;

        public string VehicleId { get; set; }
        public string Brand { get; set; }

        public int Year
        {
            get { return _year; }
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sản xuất không hợp lệ.");
                _year = value;
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                if (value < 0) throw new ArgumentException("Giá phải lớn hơn hoặc bằng 0.");
                _price = value;
            }
        }

        // Abstract methods (Bắt buộc lớp con phải triển khai)
        public abstract double CalculateMaintenanceCost();
        public abstract string GetVehicleType();

        // Virtual method (Lớp con có thể ghi đè nếu cần)
        // Polymorphism: Sẽ gọi đúng phương thức của đối tượng thực tế
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"[{GetVehicleType()}] Mã: {VehicleId} | Hãng: {Brand} | Năm: {Year} | Giá: {Price:N0} VNĐ");
            Console.WriteLine($"   => Phí bảo dưỡng: {CalculateMaintenanceCost():N0} VNĐ | Phí bảo hiểm: {CalculateInsuranceFee():N0} VNĐ");
        }

      
        public virtual double CalculateInsuranceFee()
        {
            return Price * 0.01; // Phí bảo hiểm cơ bản: 1% giá trị xe
        }
    }

   
    // 2. CÁC LỚP KẾ THỪA 
   
    public class Motorcycle : Vehicle
    {
        public int EngineCapacity { get; set; } 

        public override string GetVehicleType() => "Xe Máy";

        public override double CalculateMaintenanceCost()
        {
          
            return (Price * 0.05) + (EngineCapacity * 1000);
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   => Dung tích: {EngineCapacity}cc");
        }
    }

    public class Car : Vehicle
    {
        public int Seats { get; set; }
        public string FuelType { get; set; }

        public override string GetVehicleType() => "Ô tô con";

        public override double CalculateMaintenanceCost()
        {
            double baseMaintenance = Price * 0.07;
            if (FuelType.Equals("Điện", StringComparison.OrdinalIgnoreCase))
                return baseMaintenance * 0.8; 
            return baseMaintenance + (Seats * 50000);
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   => Số chỗ ngồi: {Seats} | Nhiên liệu: {FuelType}");
        }
    }

    public class Truck : Vehicle
    {
        public double LoadCapacity { get; set; } 

        public override string GetVehicleType() => "Xe Tải";

        public override double CalculateMaintenanceCost()
        {
            return (Price * 0.1) + (LoadCapacity * 1000000); 
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   => Tải trọng: {LoadCapacity} Tấn");
        }
    }

    public class Bus : Vehicle
    {
        public int PassengerCapacity { get; set; }

        public override string GetVehicleType() => "Xe Buýt";

        public override double CalculateMaintenanceCost()
        {
            return (Price * 0.08) + (PassengerCapacity * 200000);
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   => Sức chứa: {PassengerCapacity} người");
        }
    }

   
    // 3. LỚP QUẢN LÝ VÀ CHƯƠNG TRÌNH CHÍNH
  
    class Program
    {
        static List<Vehicle> vehicleList = new List<Vehicle>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== QUẢN LÝ PHƯƠNG TIỆN GIAO THÔNG ===");
                Console.WriteLine("1. Thêm mới phương tiện");
                Console.WriteLine("2. Hiển thị danh sách phương tiện");
                Console.WriteLine("3. Hiển thị chi tiết theo mã");
                Console.WriteLine("4. Tính tổng chi phí bảo dưỡng toàn bộ đội xe");
                Console.WriteLine("5. Tìm phương tiện theo Hãng hoặc Năm");
                Console.WriteLine("6. Xóa phương tiện theo mã");
                Console.WriteLine("7. Sắp xếp danh sách");
                Console.WriteLine("8. Thoát");
                Console.Write("Chọn chức năng (1-8): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddVehicleMenu(); break;
                    case "2": DisplayAllVehicles(); break;
                    case "3": DisplayVehicleById(); break;
                    case "4": CalculateTotalMaintenance(); break;
                    case "5": SearchVehicles(); break;
                    case "6": DeleteVehicle(); break;
                    case "7": SortVehicles(); break;
                    case "8": isRunning = false; break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại!");
                        break;
                }
                if (isRunning)
                {
                    Console.WriteLine("\nNhấn phím Enter để tiếp tục...");
                    Console.ReadKey();
                }
            }
        }

      
        // CÁC HÀM XỬ LÝ NGHIỆP VỤ & MENU
      

        static void AddVehicleMenu()
        {
            Console.Clear();
            Console.WriteLine("--- THÊM MỚI PHƯƠNG TIỆN ---");
            Console.WriteLine("1. Xe Máy");
            Console.WriteLine("2. Ô tô con");
            Console.WriteLine("3. Xe Tải");
            Console.WriteLine("4. Xe Buýt");
            Console.Write("Chọn loại xe muốn thêm (1-4): ");
            
            string typeStr = Console.ReadLine();
            Vehicle newVehicle = null;

            // Nhập thông tin chung
            string id = GetValidString("Nhập mã phương tiện (VD: XM001): ", true);
            if (vehicleList.Any(v => v.VehicleId.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Lỗi: Mã phương tiện đã tồn tại trong hệ thống!");
                return;
            }

            string brand = GetValidString("Nhập hãng sản xuất: ");
            int year = GetValidInt("Nhập năm sản xuất: ", 1900, DateTime.Now.Year);
            double price = GetValidDouble("Nhập giá mua (VNĐ): ", 0);

            switch (typeStr)
            {
                case "1":
                    int cc = GetValidInt("Nhập dung tích xi-lanh (cc, >= 50): ", 50);
                    newVehicle = new Motorcycle { EngineCapacity = cc };
                    break;
                case "2":
                    int seats = GetValidInt("Nhập số chỗ ngồi (>= 4): ", 4);
                    string fuel = GetValidString("Nhập loại nhiên liệu (Xăng/Dầu/Điện): ");
                    newVehicle = new Car { Seats = seats, FuelType = fuel };
                    break;
                case "3":
                    double load = GetValidDouble("Nhập tải trọng (tấn, >= 0.5): ", 0.5);
                    newVehicle = new Truck { LoadCapacity = load };
                    break;
                case "4":
                    int pass = GetValidInt("Nhập sức chứa hành khách (>= 16): ", 16);
                    newVehicle = new Bus { PassengerCapacity = pass };
                    break;
                default:
                    Console.WriteLine("Loại xe không hợp lệ!");
                    return;
            }

            // Gán thông tin chung
            newVehicle.VehicleId = id;
            newVehicle.Brand = brand;
            newVehicle.Year = year;
            newVehicle.Price = price;

            vehicleList.Add(newVehicle);
            Console.WriteLine("Thêm phương tiện thành công!");
        }

        static void DisplayAllVehicles()
        {
            Console.WriteLine("\n--- DANH SÁCH TẤT CẢ PHƯƠNG TIỆN ---");
            if (vehicleList.Count == 0)
            {
                Console.WriteLine("Danh sách trống.");
                return;
            }
            // POLYMORPHISM
            foreach (var vehicle in vehicleList)
            {
                vehicle.DisplayInfo();
                Console.WriteLine(new string('-', 30));
            }
        }

        static void DisplayVehicleById()
        {
            string id = GetValidString("Nhập mã phương tiện cần xem chi tiết: ");
            var vehicle = vehicleList.FirstOrDefault(v => v.VehicleId.Equals(id, StringComparison.OrdinalIgnoreCase));
            
            if (vehicle != null)
                vehicle.DisplayInfo();
            else
                Console.WriteLine("Không tìm thấy phương tiện với mã này!");
        }

        static void CalculateTotalMaintenance()
        {
            if (vehicleList.Count == 0)
            {
                Console.WriteLine("Chưa có xe nào trong hệ thống.");
                return;
            }
            double total = vehicleList.Sum(v => v.CalculateMaintenanceCost());
            Console.WriteLine($"\n=> TỔNG CHI PHÍ BẢO DƯỠNG TOÀN ĐỘI XE: {total:N0} VNĐ");
        }

        static void SearchVehicles()
        {
            Console.WriteLine("1. Tìm theo Hãng sản xuất");
            Console.WriteLine("2. Tìm theo Năm sản xuất");
            Console.Write("Chọn (1-2): ");
            string choice = Console.ReadLine();

            IEnumerable<Vehicle> results = null;

            if (choice == "1")
            {
                string brand = GetValidString("Nhập hãng sản xuất: ");
                results = vehicleList.Where(v => v.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
            }
            else if (choice == "2")
            {
                int year = GetValidInt("Nhập năm sản xuất: ");
                results = vehicleList.Where(v => v.Year == year);
            }
            else
            {
                Console.WriteLine("Lựa chọn sai.");
                return;
            }

            Console.WriteLine("\n--- KẾT QUẢ TÌM KIẾM ---");
            if (results != null && results.Any())
            {
                foreach (var v in results) v.DisplayInfo();
            }
            else
            {
                Console.WriteLine("Không tìm thấy phương tiện nào phù hợp.");
            }
        }

        static void DeleteVehicle()
        {
            string id = GetValidString("Nhập mã phương tiện cần xóa: ");
            var vehicle = vehicleList.FirstOrDefault(v => v.VehicleId.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (vehicle != null)
            {
                vehicleList.Remove(vehicle);
                Console.WriteLine("Đã xóa thành công!");
            }
            else
            {
                Console.WriteLine("Không tìm thấy phương tiện để xóa.");
            }
        }

        static void SortVehicles()
        {
            Console.WriteLine("1. Sắp xếp theo Giá (Tăng dần)");
            Console.WriteLine("2. Sắp xếp theo Năm sản xuất (Mới nhất trước)");
            Console.WriteLine("3. Sắp xếp theo Chi phí bảo dưỡng (Giảm dần)");
            Console.Write("Chọn tiêu chí sắp xếp: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    vehicleList = vehicleList.OrderBy(v => v.Price).ToList();
                    break;
                case "2":
                    vehicleList = vehicleList.OrderByDescending(v => v.Year).ToList();
                    break;
                case "3":
                    vehicleList = vehicleList.OrderByDescending(v => v.CalculateMaintenanceCost()).ToList();
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    return;
            }
            Console.WriteLine("Đã sắp xếp danh sách! Vui lòng chọn chức năng Hiển thị (2) để xem kết quả.");
        }
         
        static string GetValidString(string prompt, bool noSpaces = false)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Lỗi: Không được để trống.");
                    continue;
                }
                if (noSpaces && input.Contains(" "))
                {
                    Console.WriteLine("Lỗi: Mã không được chứa khoảng trắng.");
                    input = null; 
                }
            } while (string.IsNullOrEmpty(input));
            return input;
        }

        static int GetValidInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result >= min && result <= max)
                        return result;
                    else
                        Console.WriteLine($"Lỗi: Giá trị phải nằm trong khoảng {min} đến {max}.");
                }
                else
                {
                    Console.WriteLine("Lỗi: Vui lòng nhập một số nguyên hợp lệ.");
                }
            }
        }

        static double GetValidDouble(string prompt, double min = double.MinValue)
        {
            double result;
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out result))
                {
                    if (result >= min)
                        return result;
                    else
                        Console.WriteLine($"Lỗi: Giá trị phải lớn hơn hoặc bằng {min}.");
                }
                else
                {
                    Console.WriteLine("Lỗi: Vui lòng nhập một số hợp lệ.");
                }
            }
        }
    }
}