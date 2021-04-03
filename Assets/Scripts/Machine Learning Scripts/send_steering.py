import socket
import time
from tensorflow import keras
import numpy as np
from pickle import load


model = keras.models.load_model("./nomalized_model.h5")
scaler = load(open('scaler.pkl', 'rb'))

host = "192.168.0.40"
port = 1234
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((host, port))

while True:
    time.sleep(0.05)
    recievedData = s.recv(1024).decode("UTF-8")
    recievedData = recievedData.split(",")
    recievedData = list(map(float, recievedData))

    if recievedData:
        print("Recieved: " + str(recievedData))
        prediction = str(np.argmax(model.predict([scaler.transform([recievedData])])))
        # prediction = str(np.argmax(model.predict([recievedData])))
        print("Sending: " + prediction)
        s.sendall(prediction.encode("UTF-8"))
