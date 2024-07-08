import React, { useEffect, useState } from "react";
import { v4 as uuidv4 } from "uuid";
import {
  HubConnectionBuilder,
  HubConnectionState,
  HubConnection,
  HttpTransportType,
} from "@microsoft/signalr";

const App = () => {
  const [customers, setCustomers] = useState([]);
  const [customers2, setCustomers2] = useState([]);
  const [notification, setNotification] = useState([]);

  const convertToLowerCase = (obj) => {
    var newObj = {};
    for (var key in obj) {
      if (obj.hasOwnProperty(key)) {
        var lowerKey = key.toLowerCase();
        var value = obj[key];
        newObj[lowerKey] = value;
      }
    }
    return newObj;
  };

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:5001/notification", {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();
    const startConnection = async () => {
      connection.on("ReceivedTableData", (data) => {
        setCustomers([...customers, ...data]);
      });

      connection.on("ReceivedNotifications", (message) => {
        const data = JSON.parse(message);

        const convertedData = convertToLowerCase(data.Data);

        if (convertedData.id > 0)
          setNotification([...notification, convertedData]);
        else {
          setCustomers((prevCustomers) =>
            prevCustomers.filter(
              (item) => item.id !== Math.abs(convertedData.id)
            )
          );
        }
      });

      try {
        await connection.start();
        await connection.invoke("GetTableData");
      } catch (err) {
        console.error("Error establishing connection:", err);
      }
    };

    startConnection();

    return () => connection.stop();
  }, []);

  useEffect(() => {
    notification.forEach((obj2) => {
      const index = customers.findIndex((obj1) => obj1.id === obj2.id);
      
      // If id is found in arr1, update the object
      if (index !== -1) {
        const updatedArr1 = [...customers];
        updatedArr1[index] = obj2;
        setCustomers(updatedArr1);
      } 
      
      // If id is not found in arr1, append the object
      else {
        setCustomers((prevArr1) => [...prevArr1, obj2]);
      }
      
    });
  }, [notification]);

  const sortCustomersById = () => {
    if (customers) {
      const sortedCustomers = [...customers].sort((a, b) => {
        return a.id - b.id;
      });
      setCustomers(sortedCustomers);
    }
  };

  // Initial sorting when component mounts
  useEffect(() => {
    sortCustomersById();
  }, []);

  return (
    <div>
      <h2 style={{ textAlign: "left", marginBottom: "20px" }}>
        Notification Table
      </h2>
      <table style={{ width: "30%", borderCollapse: "collapse", border: "1px solid black" }}>
        <thead>
          <tr style={{ backgroundColor: "#f2f2f2" }}>
            <th
              style={{
                padding: "10px",
                textAlign: "left",
                borderBottom: "1px solid #ddd",
              }}
            >
              ID
            </th>
            <th
              style={{
                padding: "10px",
                textAlign: "left",
                borderBottom: "1px solid #ddd",
              }}
            >
              Name
            </th>
          </tr>
        </thead>
        <tbody>
          {customers.length > 0 &&
            customers.map((customer) => (
              <tr key={uuidv4()} style={{ borderBottom: "1px solid #ddd" }}>
                <td style={{ padding: "10px", textAlign: "left" }}>
                  {customer.id}
                </td>
                <td style={{ padding: "10px", textAlign: "left" }}>
                  {customer.name}
                </td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default App;
