create table Payments(
Id int identity(1,1) primary key,
PaymentName nvarchar(30)
);

create table OrderPayment(
Id int identity(1,1) primary key,
CreditCardNumber varchar(16),
SecureCode varchar(3),
CardOwnerName nvarchar(100),
OrderId int,
PaymentId int,
constraint FK_OrderPayment_Order foreign key (OrderId) references [Order](Id),
constraint FK_OrderPayment_Payments foreign key (PaymentId) references Payments(Id)
);

insert into Payments values
('Kreditkarte'),
('Überweisung');