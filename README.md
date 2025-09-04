# 3D Container Loading Problem with Variable Neighborhood Search

Bu proje, 3 boyutlu konteyner yerleştirme problemini (3D Packing / Container Loading Problem) çözmek amacıyla geliştirilmiştir. Problem, farklı boyutlardaki kutuların belirli kurallara göre bir konteynere en verimli şekilde yerleştirilmesini hedefler.

## Özellikler

- **Çözüm Yöntemi:**  
  Variable Neighborhood Search (VNS) algoritması kullanılarak meta-sezgisel bir çözüm yaklaşımı geliştirilmiştir.

- **Görselleştirme:**  
  Python dili kullanılarak kutu yerleşimlerini görsel olarak inceleme imkânı sağlayan bir debug arayüzü mevcuttur.

- **Performans:**  
  Uygulama, literatürde yer alan bazı önceki çalışmalardan daha iyi sonuçlar elde etmektedir.

## Proje Yapısı

- `VNS3/`  
  VNS algoritması ile çözüm üreten C# kodlarını içerir.

- `debug/`  
  Python ile geliştirilmiş yerleşim görselleştirme arayüzünü içerir.

## Kullanım

1. Depoyu klonlayın:
   ```bash
   git clone https://github.com/megfb/3dPacking---Containler-Loading-Problem.git
