import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import numpy as np
import random
import os
# Python dosyasının bulunduğu dizini al
current_dir = os.path.dirname(os.path.abspath(__file__))
number = input("insert a problem number")
# txt dosyasının yolunu oluştur
file_path = os.path.join(current_dir, 'boxes'+number+'.txt')

# Dosyayı aç ve içeriğini oku
with open(file_path, 'r', encoding='utf-8') as file:
    content = file.read()

# İçeriği listeye dönüştür
boxes = eval(f'[{content}]')


fig = plt.figure(figsize=(50, 40))
ax = fig.add_subplot(111, projection='3d')

# Rastgele renkler oluştur
colors = ["red", "green", "blue", "yellow", "purple", "orange", "cyan", "magenta", "lime", "pink"]
opacity = 1
# Her kutu için çizim yap
for name, (x, y, z), width, depth, height in boxes:
    color = random.choice(colors)  # Rastgele bir renk seç

    # Alt ve üst yüzeyler
    X, Y = np.meshgrid([x, x+width], [y, y+depth])
    Z = np.array([[z, z], [z, z]])
    ax.plot_surface(X, Y, Z, color=color, alpha=opacity)  # Alt yüzey
    ax.plot_surface(X, Y, Z+height, color=color, alpha=opacity)  # Üst yüzey

    # Ön ve arka yüzeyler
    X, Z = np.meshgrid([x, x+width], [z, z+height])
    Y = np.array([[y, y], [y, y]])
    ax.plot_surface(X, Y, Z, color=color, alpha=opacity)  # Ön yüzey
    ax.plot_surface(X, Y+depth, Z, color=color, alpha=opacity)  # Arka yüzey

    # Yan yüzeyler
    Y, Z = np.meshgrid([y, y+depth], [z, z+height])
    X = np.array([[x, x], [x, x]])
    ax.plot_surface(X, Y, Z, color=color, alpha=opacity)  # Sol yüzey
    ax.plot_surface(X+width, Y, Z, color=color, alpha=opacity)  # Sağ yüzey

ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_zlabel('Z')

plt.show()
