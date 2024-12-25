insert into AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
Values('25501994-44dd-44b8-bb7d-1b2af376f1be','Admin','Admin', '1')
GO

insert into AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
Values('32b89678-1f5d-43c8-8dbd-4251902bdfa4','Trainer','Trainer', '2')
GO

insert into AspNetRoles(Id, Name, NormalizedName, ConcurrencyStamp)
Values('345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7','Student','Student', '3')
GO

-- app user for trainer

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://external-preview.redd.it/4VOfhCdsYxUk7Q02PQXE3PJj4FAuwX8GukT2RQcI_BY.jpg?auto=webp&s=e6299c1fd5eef10ad133b5573d64b25291703991','08eec65c-8aab-4499-a57f-52cf0486c35a', 'Jay Cutler', 'Sterling, Massachusetts, United States', 0, '$2a$04$Y//wUiZo3.386/hoD739cOpSqQWtds45BeMF3SXWLYbXGR34l9o5a', '$2a$04$Ec5qIr/TH7WArMW9P6Cwi.iMkfzWl.aR6gBjANxFjUYvX2dVayB5S', '286 718 9312', 0, 0, 1, 0, 'Jay_Cutler@typepad.com', 'Jay_Cutler@TYPEPAD.COM', 'JAY_CUTLER@TYPEPAD.COM', 'JAY_CUTLER@TYPEPAD.COM');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSMQWC4lsy3HMlnOtN4ZMCL3oLbCRLf8bC3hQ&s','43328814-f5f0-4d9f-ba58-f4a74331f51a', 'Dwayne "The Rock" Johnson', 'Hayward, California, United States', 0, '$2a$04$tiSCWPKi0EAg4I.YqXD3s.XRt0SKVy4a1Vgn139r8VA/afMfVzGxC', '$2a$04$Skn9mSp2u4wG5ifeOmu.ReqiTjyV.iHC7AgK9GL/uugZjDZh/kQEa', '673 789 7777', 0, 0, 1, 0, 'Dwayne_Johnson@tuttocitta.it', 'Dwayne_Johnson@TUTTOCITTA.IT', 'DWAYNE_JOHNSON@TUTTOCITTA.IT', 'DWAYNE_JOHNSON@TUTTOCITTA.IT');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://i.redd.it/xokw9t7khfv41.jpg','2343b181-ba69-431f-ad8d-e9b9efaf518b', 'Chris Bumstead', 'Ottawa, Canada', 0, '$2a$04$gF8I3aNJYO8oGoOb3DS4S.plFjJpr1uxBCzWcXrIPdSSof7L6sm5i', '$2a$04$tqCHglUggjNuonjt781oVOLraHSY6WbwCzbZyqWxSV0Wu4vgUyfae', '231 554 5625', 0, 0, 1, 0, 'Chris_Bumstead@ocn.ne.jp', 'Chris_Bumstead@OCN.NE.JP', 'CHRIS_BUMSTEAD@OCN.NE.JP', 'CHRIS_BUMSTEAD@OCN.NE.JP');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/Phil_Heath.JPG/640px-Phil_Heath.JPG','49fe1a18-c93d-4f94-b28e-121db0ea7e18', 'Phil Heath', 'Seattle, Washington, United States', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '504-603-9370', 0, 0, 1, 0, 'Phil_Heath@gmail.com', 'Phil_Heath@gmail.com', 'PHIL_HEATH@gmail.com', 'PHIL_HEATH@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://i.pinimg.com/736x/59/b7/39/59b73905d7b67a747bdc5182d4c4fd89.jpg','49fe1a18-c93d-4f94-b28e-121db0eee18a', 'Kai Greene', 'Brooklyn, New York, United States', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '891-121-5823', 0, 0, 1, 0, 'Kai_Greene@gmail.com', 'Kai_Greene@gmail.com', 'KAI_GREENE@gmail.com', 'KAI_GREENE@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://i.pinimg.com/736x/d3/fc/a0/d3fca0e0e8742f8f480f6ce8bb0b7790.jpg','49fe1a18-dhyu-4f94-b28e-121db0ea7e18', 'Jeff Cavaliere', 'Connecticut, USA', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '651-213-7918', 0, 0, 1, 0, 'Jeff_Cavaliere@gmail.com', 'Jeff_Cavaliere@gmail.com', 'JEFF_CAVALIERE@gmail.com', 'JEFF_CAVALIERE@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://www.greatestphysiques.com/wp-content/uploads/2017/07/Mike-Rashid.07.jpg','49fe1a18-aa3d-4f94-b28e-121db0ea7e18', 'Mike Rashid', 'Brooklyn, New York, USA', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '316-899-9888', 0, 0, 1, 0, 'Mike_Rashid@gmail.com', 'Mike_Rashid@gmail.com', 'MIKE_RASHID@gmail.com', 'MIKE_RASHID@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://muscleinsider.com/sites/default/files/styles/node_gallery_display/public/CALUM_VON_MOGER_UNDERGOES_SPINAL_SURGERY.png?itok=fMDVa9Mr','49fe1h68-c93d-4f94-b28e-121db0ea7e18', 'Calum Von Moger', 'Victoria, Australia', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '502-694-1145', 0, 0, 1, 0, 'Calum_Von_Moger@gmail.com', 'Calum_Von_Moger@gmail.com', 'CALUM_VON_MOGER@gmail.com', 'CALUM_VON_MOGER@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQhboK_eKhXhrqCZBfBG8qCnWaECGHWpUPMvA&s','49fe1a18-c93d-4f94-t28e-121db0ea7e18', 'Steve Cook', 'Boise, Idaho, United States', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '170-429-3567', 0, 0, 1, 0, 'Steve_Cook@gmail.com', 'Steve_Cook@gmail.com', 'STEVE_COOK@gmail.com', 'STEVE_COOK@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://i.pinimg.com/736x/87/99/4b/87994be3929439a90bc166c67639678a.jpg','49fe1a18-c93d-4f94-b44e-121db0ea7e18', 'Dana Linn Bailey', 'Reading, Pennsylvania, United States', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '305-699-1502', 0, 0, 1, 0, 'Dana_Linn_Bailey@gmail.com', 'Dana_Linn_Bailey@gmail.com', 'DANA_LINN_BAILEY@gmail.com', 'DANA_LINN_BAILEY@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://www.greatestphysiques.com/wp-content/uploads/2017/05/bradley-martyn-besides-dumbbell-rack-looking-at-himself-in-the-mirror.jpg','49fe1a18-c93d-4f94-b28e-121dmkka7e18', 'Bradley Martyn', 'San Francisco Bay Area, California, United States', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '216-964-1207', 0, 0, 1, 0, 'Bradley_Martyn@gmail.com', 'Bradley_Martyn@gmail.com', 'BRADLEY_MARTYN@gmail.com', 'BRADLEY_MARTYN@gmail.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://yt3.googleusercontent.com/ytc/AIdro_mekKzu4XyG9j_wjS6DSLslac3O-JUbO7DJQSZEzXPe7-M=s900-c-k-c0x00ffffff-no-rj','49fe1a18-c93d-4f94-hjki-121db0ea7e18', 'Lazar Angelov', 'Sofia, Bulgaria', 0, '$2a$04$iPUjTFL/Fv8uqrhNPdzZVuWemR0/H8zvLNalDANdYHPQ86Q5bOGJy', '$2a$04$ePVDc9.gB7rxE7jvdMCauOveDmBekFXqOUYjUFLYD9qWVB3LMo77O', '998-185-5096', 0, 0, 1, 0, 'Lazar_Angelov@gmail.com', 'Lazar_Angelov@gmail.com', 'LAZAR_ANGELOV@gmail.com', 'LAZAR_ANGELOV@gmail.com');
GO

-- app user for student
insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://static1.squarespace.com/static/656f4e4dababbd7c042c4946/657236350931ee4538eea52c/65baf15103d8ad2826032a8a/1727029299965/how-to-stop-being-a-people-pleaser-1_1.jpg?format=1500w','2f0df4d2-9e63-4ff6-a8d2-96617f115f59', 'Bertha Zemlak', '77868 Emory Ports',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '538-738-5038', 0, 0, 1, 0, 'Bertha_Zemlak@gmail.com', 'Bertha_Zemlak@gmail.com', 'BERTHA_ZEMLAK@GMAIL.com', 'BERTHA_ZEMLAK@GMAIL.com');

GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://politics.princeton.edu/sites/default/files/styles/square/public/images/p-5.jpeg?h=87dbaab7&itok=ub6jAL5Q','bed0536f-6c4f-4c00-babd-98eb676aedba', 'Brandi Hettinger', '124 Lesly Spring',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '421-882-8095', 0, 0, 1, 0, 'Brandi_Hettinger@gmail.com', 'Brandi_Hettinger@gmail.com', 'BRANDI_HETTINGER@GMAIL.com', 'BRANDI_HETTINGER@GMAIL.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://static.vecteezy.com/system/resources/thumbnails/004/834/925/small_2x/multiracial-group-of-young-people-taking-selfie-photo.jpg','5b832f49-f67f-49d5-b3a3-1252a21cb38e', 'Jenna Sawayn', '270 Malachi Squares',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '990-287-4674', 0, 0, 1, 0, 'Jenna_Sawayn@gmail.com', 'Jenna_Sawayn@gmail.com', 'JENNA_SAWAYN@GMAIL.com', 'JENNA_SAWAYN@GMAIL.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://cdn-01.cms-ap-v2i.applyflow.com/pinnacle-people/wp-content/uploads/2023/09/slide-2.png','2c67dfea-8fec-4c6f-b4ef-a887bd41f008', 'Casey Dooley', '977 Josiane Route',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '930-413-6602', 0, 0, 1, 0, 'Casey_Dooley@gmail.com', 'Casey_Dooley@gmail.com', 'CASEY_DOOLEY@GMAIL.com', 'CASEY_DOOLEY@GMAIL.com');

GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://people.com/thmb/sEU4QSHyCBepK9-6JwNVUGZAgtY=/4000x0/filters:no_upscale():max_bytes(150000):strip_icc():focal(449x0:451x2)/people-headshot-lauren-lieberman-830b33fdd4cc4c4bbc6e71ebd84dd633.jpg','ed5bd378-d7f2-45c6-acb7-32990fff3752', 'Mrs. Lisa Windler', 'Justina Forges',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '351-109-2179', 0, 0, 1, 0, 'Lisa_Windler@gmail.com', 'Lisa_Windler@gmail.com', 'LISA_WINDLER@GMAIL.com', 'LISA_WINDLER@GMAIL.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://www.covenanthouse.org/sites/default/files/styles/805x740/public/2023-08/bipoc-youth.jpg.webp?itok=0gtkoEHM','b9ea1881-57e1-469a-a2dd-9922a05994bf', 'Darin Connell', '6843 Cordell Mount',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '924-241-4353', 0, 0, 1, 0, 'Darin_Connell@gmail.com', 'Darin_Connell@gmail.com', 'DARIN_CONNELL@GMAIL.com', 'DARIN_CONNELL@GMAIL.com');
GO

insert into AspNetUsers (PasswordHash,Avatar,Id, FullName, Address, EmailConfirmed, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Email, UserName, NormalizedUserName, NormalizedEmail) values ('AQAAAAIAAYagAAAAEMUFKxmZVBJLRy0ryQUqzepEyhBKE1sNTHTJ33Vw9Sz1CT8tcO5sQ2Zc9pYH8VCIXA==','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTcj2_Ehd24LTZspXISSGHMQVDj8syMNso6dQ&s','122a05cc-c431-424d-b528-c6c8b5a0883a', 'Levi Reynolds', '5704 Earl Knoll',0, '$2a$04$UlBYwaI/G7PP.vo3lhh48OI4bP4OTzPqKX9/XHBsC2HdZ0nrv2rOa', '$2a$04$BG5KC/6R7jM2YZNdtGzlCeVw5nJ.tI0ItOpfzCvcD6e6F4u2noWb2', '584-695-6606', 0, 0, 1, 0, 'Levi_Reynolds@gmail.com', 'Levi_Reynolds@gmail.com', 'LEVI_REYNOLDS@GMAIL.com', 'LEVI_REYNOLDS@GMAIL.com');


-- add role to trainer
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-b28e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-b28e-121dmkka7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-hjki-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-b28e-121db0eee18a','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('43328814-f5f0-4d9f-ba58-f4a74331f51a','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('08eec65c-8aab-4499-a57f-52cf0486c35a','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-aa3d-4f94-b28e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1h68-c93d-4f94-b28e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-dhyu-4f94-b28e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-t28e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('49fe1a18-c93d-4f94-b44e-121db0ea7e18','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('2343b181-ba69-431f-ad8d-e9b9efaf518b','32b89678-1f5d-43c8-8dbd-4251902bdfa4')
go


-- add role to student
Insert Into AspNetUserRoles(UserId, RoleId) Values('2f0df4d2-9e63-4ff6-a8d2-96617f115f59','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('b9ea1881-57e1-469a-a2dd-9922a05994bf','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('2c67dfea-8fec-4c6f-b4ef-a887bd41f008','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('122a05cc-c431-424d-b528-c6c8b5a0883a','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('5b832f49-f67f-49d5-b3a3-1252a21cb38e','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('bed0536f-6c4f-4c00-babd-98eb676aedba','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go
Insert Into AspNetUserRoles(UserId, RoleId) Values('ed5bd378-d7f2-45c6-acb7-32990fff3752','345996a0-0f9e-4f4e-a7a5-1cbc7a110cc7')
go


-- Trainer
insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('08eec65c-8aab-4499-a57f-52cf0486c35a','https://external-preview.redd.it/4VOfhCdsYxUk7Q02PQXE3PJj4FAuwX8GukT2RQcI_BY.jpg?auto=webp&s=e6299c1fd5eef10ad133b5573d64b25291703991','Jay_Cutler@typepad.com','Jay Cutler', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Triple-buffered 5th generation encryption', 'Voluptatem enim et quibusdam perferendis deserunt nesciunt quam est fugit.', 30, 3, '2023-03-18', '2024-12-13');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('2343b181-ba69-431f-ad8d-e9b9efaf518b','https://i.redd.it/xokw9t7khfv41.jpg','Chris_Bumstead@ocn.ne.jp','Chris Bumstead', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Organic client-server Graphical User Interface', 'Nemo dolore repudiandae necessitatibus nulla possimus labore quod quo cupiditate.', 37, 0, '2023-03-05', '2024-12-14');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('43328814-f5f0-4d9f-ba58-f4a74331f51a','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSMQWC4lsy3HMlnOtN4ZMCL3oLbCRLf8bC3hQ&s','Dwayne_Johnson@tuttocitta.it','Dwayne "The Rock" Johnson', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Seamless 4th generation application', 'Exercitationem et eos reiciendis aliquid error aut quam et aliquam.', 17, 5, '2024-09-22', '2024-11-28');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-aa3d-4f94-b28e-121db0ea7e18','https://www.greatestphysiques.com/wp-content/uploads/2017/07/Mike-Rashid.07.jpg','Mike_Rashid@gmail.com','Mike Rashid', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Distributed composite standardization', 'Et cum sed labore similique asperiores impedit ducimus et ipsam.', 40, 3, '2023-11-18', '2024-12-19');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-b28e-121db0ea7e18','https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/Phil_Heath.JPG/640px-Phil_Heath.JPG','Phil_Heath@gmail.com','Phil Heath', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Operative zero tolerance project', 'Porro eaque ut possimus quam iure qui veniam quia aperiam.', 30, 4, '2024-09-02', '2024-11-25');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-b28e-121db0eee18a','https://i.pinimg.com/736x/59/b7/39/59b73905d7b67a747bdc5182d4c4fd89.jpg','Kai_Greene@gmail.com','Kai Greene', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Front-line human-resource migration', 'Nobis impedit debitis optio non atque quia ut nihil est.', 26, 3, '2023-02-11', '2024-12-19');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-b28e-121dmkka7e18','https://www.greatestphysiques.com/wp-content/uploads/2017/05/bradley-martyn-besides-dumbbell-rack-looking-at-himself-in-the-mirror.jpg','Bradley_Martyn@gmail.com','Bradley Martyn', '39612469-851d-456c-bf74-d0627821e490', 'Optimized attitude-oriented encoding', 'Assumenda porro consequuntur animi sapiente id voluptatem nostrum doloribus dolor.', 10, 3, '2024-07-02', '2024-12-17');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-b44e-121db0ea7e18','https://i.pinimg.com/736x/87/99/4b/87994be3929439a90bc166c67639678a.jpg','Dana_Linn_Bailey@gmail.com','Dana Linn Bailey', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Face to face 5th generation budgetary management', 'Voluptas ducimus omnis quo possimus possimus quas deserunt omnis reiciendis.', 42, 5, '2024-05-19', '2024-12-05');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-hjki-121db0ea7e18','https://yt3.googleusercontent.com/ytc/AIdro_mekKzu4XyG9j_wjS6DSLslac3O-JUbO7DJQSZEzXPe7-M=s900-c-k-c0x00ffffff-no-rj','Lazar_Angelov@gmail.com','Lazar Angelov', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Open-source asymmetric workforce', 'Ea et quo est asperiores eos sint et fugiat eius.', 15, 3, '2024-06-26', '2024-12-07');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-c93d-4f94-t28e-121db0ea7e18','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQhboK_eKhXhrqCZBfBG8qCnWaECGHWpUPMvA&s','Steve_Cook@gmail.com','Steve Cook', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Object-based transitional budgetary management', 'Maiores enim dignissimos architecto mollitia et facere velit dolore maiores.', 21, 5, '2023-11-03', '2024-12-05');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1a18-dhyu-4f94-b28e-121db0ea7e18','https://i.pinimg.com/736x/d3/fc/a0/d3fca0e0e8742f8f480f6ce8bb0b7790.jpg','Jeff_Cavaliere@gmail.com','Jeff Cavaliere', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'Organized intermediate concept', 'Voluptatem adipisci assumenda sed quas tenetur repellat consequatur enim sint.', 31, 1, '2023-09-13', '2024-11-23');
GO

insert into Trainers (UserId,Profile_Picture, Email, Name, Trainer_ID, Certification, Bio, Experience, Rating, Desciption, Created_At, Updated_At) values ('49fe1h68-c93d-4f94-b28e-121db0ea7e18','https://muscleinsider.com/sites/default/files/styles/node_gallery_display/public/CALUM_VON_MOGER_UNDERGOES_SPINAL_SURGERY.png?itok=fMDVa9Mr','Calum_Von_Moger@gmail.com','Calum Von Moger', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Upgradable well-modulated budgetary management', 'Nemo enim et adipisci quo voluptatem rerum quas dicta minima.', 15, 4, '2023-03-07', '2024-12-17');
GO

-- student
Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('aaba1a14-3d5e-4375-9881-7a370bc7b6ee', 'Levi Reynolds', 'Levi_Reynolds@gmail.com', '', 'Good', 'https://news.uchicago.edu/sites/default/files/styles/square_feature/public/images/2023-10/Adam-Mastroianni-square.jpg?h=daa376fd&itok=YR0-YXHv','2024-09-27','2024-12-12','122a05cc-c431-424d-b528-c6c8b5a0883a')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('6f4a29b4-0127-42ff-b92e-08ef42eaf2e0', 'Casey Dooley', 'Casey_Dooley@gmail.com', '', 'Good', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSzmhpaQ5r2ctu0U4iFasLVOViEunegjZD6Hg&s','2024-07-14','2024-12-09','2c67dfea-8fec-4c6f-b4ef-a887bd41f008')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('1dc98a6a-980d-416e-8012-15d1dd14f053', 'Bertha Zemlak', 'Bertha_Zemlak@gmail.com', '', 'Good', 'https://i.pinimg.com/736x/cd/26/fb/cd26fb2ba208f741fbd0a8e5d980b895.jpg','2023-10-26','2024-12-17','2f0df4d2-9e63-4ff6-a8d2-96617f115f59')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('b763a0a6-a445-457b-bc0e-30e02772d36d', 'Jenna Sawayn', 'Jenna_Sawayn@gmail.com', '', 'Good', 'https://i.pinimg.com/736x/2c/dd/1b/2cdd1b72e1334717eed7374a42b39889.jpg','2024-01-15','2024-11-30','5b832f49-f67f-49d5-b3a3-1252a21cb38e')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('2b90afd9-35e2-4087-b656-2efdac7dc7a6', 'Darin Connell', 'Darin_Connell@gmail.com', '', 'Good', 'https://i.pinimg.com/736x/5f/dd/34/5fdd34bed9b1197a622b1d175ebc03f3.jpg','2024-06-26','2024-12-18','b9ea1881-57e1-469a-a2dd-9922a05994bf')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('baaf88a2-bf03-4b26-bf48-9a3e6c6db544', 'Brandi Hettinger', 'Brandi_Hettinger@gmail.com', '', 'Good', 'https://i.pinimg.com/736x/f6/2f/a8/f62fa84d75c7a0a5cb4e2473b4d42199.jpg','2023-11-12','2024-11-23','bed0536f-6c4f-4c00-babd-98eb676aedba')
GO

Insert Into Students (Student_ID, Name, Email, Password, Health_Status, Profile_Picture,Created_At,Updated_At,UserId)
Values ('f8f3a329-a11b-4de6-8e38-2d3fb9361c66', 'Mrs. Lisa Windler', 'Lisa_Windler@gmail.com', '', 'Good', 'https://i.pinimg.com/736x/38/c2/32/38c23251c49bd5a97da8810fbccf0823.jpg','2023-09-15','2024-11-25','ed5bd378-d7f2-45c6-acb7-32990fff3752')
GO

-- course
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b8315209-e30a-4268-b8f4-136c7d821f8e', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Types of Workouts: A Guide to Finding the Best Exercise for You', 'Cross fit','Cross fit', 769376, 392027,'2024-02-23','2024-11-27',35,8,4, 'In today’s fitness-focused world, understanding the various types of workouts available is essential for achieving your health and fitness goals  Whether you&#8217;re aiming for weight loss, muscle gain, improved endurance, or overall well-being, there&#8217;s a workout tailored for you  This comprehensive guide will delve into the different types of workouts, their benefits, examples, and considerations for who might need to consult a professional before starting 
1  Cardiovascular Workouts
Cardiovascular workouts, also known as aerobic exercises, are designed to get your heart rate up and improve cardiovascular health')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('42971518-9fd1-4712-b496-71f1ca1a2aaf', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'The Power of Sprinting: Unleashing the King of Exercises for Optimal Fitness', 'Strength training','Strength training', 8659, 19471,'2024-05-30','2024-11-29',38,7,4, 'When it comes to exercise, there&#8217;s one activity that stands out for its incredible benefits, yet many people overlook it in their workout routines  We&#8217;re talking about sprinting – a primordial form of exercise that can transform your fitness journey in remarkable ways  In this blog article, we&#8217;ll explore the unique advantages of sprinting, its impact on various aspects of health, and why it could be rightfully called the king of exercises 
The Power of Sprinting: Making You Strong, Fast, and Lean
1  The Intensity Factor: Strengthening Your Body
When you exercise, your body responds by adapting to the challenges you impose')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('c0627319-216b-4740-afb4-ec2ccf480a2f', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'What is The Best Time to Workout: Understanding Circadian Rhythm and Hormonal Impact', 'Pilates','Pilates', 1777287, 1244349,'2023-09-15','2024-11-27',38,4,4, 'Embarking on a weight loss journey is often accompanied by enthusiasm and dedication  However, as time goes on, you might find yourself hitting a frustrating roadblock known as the weight loss plateau  In this comprehensive guide, we will delve into the reasons behind a weight loss plateau and equip you with effective strategies to overcome it  Let&#8217;s explore how to reignite your progress and achieve your desired goals 
Understanding the Weight Loss Plateau:
After weeks or months of steady progress, it&#8217;s not uncommon to suddenly see the numbers on the scale stagnate')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('24541e22-7217-49de-8347-19446c36fa49', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'What Percentage of Weight Loss is Diet vs Exercise?', 'Strength training','Strength training', 1208432, 217901,'2023-11-19','2024-12-01',4,5,4, 'The pursuit of weight loss often prompts the question: which factor contributes more significantly, diet or exercise? This query reflects the intricate interplay between two crucial elements of a healthy lifestyle  While both diet and exercise play pivotal roles in achieving weight loss goals, understanding their respective influences can guide individuals towards a balanced approach that optimizes results  In this article, we explore the percentages and nuances of weight loss attributed to diet and exercise 
Diet vs  Exercise: The Numbers Game:

 The Importance of Diet:

Numerous studies suggest that weight loss is predominantly influenced by dietary choices')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('904db638-7bc3-481a-8fd6-1657862cf8ec', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Breaking Through the Weight Loss Plateau: Strategies for Success', 'Cross fit','Cross fit', 1600186, 1798361,'2024-09-25','2024-12-16',41,2,2, 'Welcome to our guide on how to improve your strength! Whether you&#8217;re a beginner or an experienced fitness enthusiast, this article is for you  In this guide, we&#8217;ll cover various ways to improve your strength, including lifting weights, doing bodyweight exercises, and practising yoga  We&#8217;ll also provide tips on nutrition and hydration for optimal strength gains  By the end of this article, you&#8217;ll have all the information you need to get started on your strength training journey 
What Is Strength?

Strength is the ability of a muscle or group of muscles to exert force against a resistance')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('dc74d51d-9b27-4513-845e-be51961068d2', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Building Muscle with Bodyweight Exercises: A Beginners Guide', 'Pilates','Pilates', 101032, 260122,'2024-05-11','2024-12-19',2,5,0, 'If you&#8217;re new to fitness and want to build muscle, you might assume that you need to hit the gym and lift weights  While weight training is an effective way to build muscle, it&#8217;s not the only option  Bodyweight exercises, which use your own body weight as resistance, can also be a great way to build strength and muscle mass 
In this article, we&#8217;ll explore the science behind building muscle with bodyweight exercises, the benefits of bodyweight exercises, provide a list of must-do exercises, and offer tips on how to structure your workout routine for optimal results  We&#8217;ll also answer some frequently asked questions about building muscle with bodyweight exercises')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('86b7c89c-a188-4628-a846-5636c1b28698', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'How To Improve Strength: A Beginners Guide', 'Cardio', 'Cardio', 389125, 861193,'2023-03-30','2024-12-04',2,4,1, 'Have you ever wondered if there is an ideal time to work out that maximises the benefits of exercise? As it turns out, our bodies have their own internal clocks, known as circadian rhythms, which are influenced by three essential hormones: cortisol, testosterone, and melatonin  In this article, we will delve into the fascinating interplay of these hormones, discover the concept of the testosterone to cortisol ratio, and determine the optimal time to optimise your workout routine for better results  
Circadian Rhythm and Hormonal Impact
Our body&#8217;s circadian rhythm serves as its internal timekeeper, governing various physiological processes and behaviours throughout a 24-hour cycle  This rhythm is regulated by three key hormones: cortisol, testosterone, and melatonin 
Melatonin: The Sleep Hormone
Melatonin is commonly known as the &#8220;sleep hormone')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b5733f63-50b0-4b27-a6d3-f80f8be9e1f8', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Resistance Training Exercise for Diabetes: A Beginners Guide', 'Pilates','Pilates', 1943841, 102501,'2023-10-05','2024-12-02',34,3,1, 'Pregnancy is an exciting time, but it can also be challenging for the female body  Eating well and staying physically active are both important to support the changes that happen during this time  But when it comes to exercise, there are a lot of myths and misconceptions that can make it difficult to know what to do  In this article, we&#8217;ll cover everything you need to know about exercise during pregnancy, including weight lifting, Kegel exercises, breathing exercises, and more 
What Special Attention To Health Is Needed During Pregnancy?
Pregnancy is a crucial phase in a woman&#8217;s life that involves significant physical and emotional changes')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('5b7d3bfb-18e2-4ed0-bf21-75bddc36cdda', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Functional Training for Gym Trainees: A Beginners Guide to Building Strength', 'Pilates','Pilates', 1051953, 1715599,'2023-04-02','2024-11-24',46,6,4, 'Many people believe that sweating is a sign of a good workout and that it leads to weight loss  But is this really true? In this article, we will explore whether sweating can help you lose fat, and whether it has any impact on belly fat  We will also discuss the benefits of sweating for weight loss 
What Is Sweat?
Sweat is a natural bodily function produced by sweat glands in response to various factors such as heat, exercise, and stress  Sweat is composed of water, electrolytes, and small amounts of waste products such as urea')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('3aeb3b7e-1211-4cd0-a566-6192528a1315', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Exercise During Pregnancy: What You Need to Know', 'Pilates','Pilates', 1556535, 554826,'2023-10-15','2024-12-05',43,6,2, 'Are you one of the millions of people living with type 2 diabetes? If so, you&#8217;ve probably been told that increasing your physical activity and exercise can help manage your condition  But did you know that resistance training can have a significant impact on your blood glucose levels?
In this article, we&#8217;ll explore how resistance training can benefit diabetes patients, how to get started with a resistance training program, and how to incorporate resistance training into an overall exercise plan  By the end, you&#8217;ll have the tools you need to manage your diabetes with confidence and ease 
What Is Diabetes?
Diabetes is a chronic condition characterized by high blood glucose levels  It occurs when the body is unable to produce or use insulin effectively')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d28f314b-1fab-423e-bd22-734dfbf93b7e', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'Can Sweating Help You Lose Fat?', 'Strength training','Strength training', 280145, 1700519,'2023-12-18','2024-12-07',26,1,4, 'If you&#8217;re new to fitness, building a strong and muscular back can seem like a daunting task  However, with the right exercises and techniques, anyone can develop a buffed back  In this article, we&#8217;ll cover everything you need to know to get started, including recommended exercises, tips for proper form, and more 
Introduction To Your Back Muscles
The back is a complex muscle group composed of several muscles that work together to support the spine and facilitate movement of the upper body  Here are the major muscles of the back:

Trapezius: The trapezius is a large, triangular-shaped muscle that spans the upper back, neck, and shoulders')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('0a800eea-7b71-49ce-8c11-94b91346d8ec', '0c96dc20-9ce7-4c1f-a3cc-7f7f648eb6be', 'How to Build a Buffed Back: The Beginners Back Workout Guide', 'Pilates','Pilates', 1594485, 1514064,'2023-07-03','2024-12-11',14,2,2, 'Functional strength training is an effective and efficient way for beginners to improve overall fitness, mobility, and stability while reducing the risk of injury  It offers a full-body workout that mimics daily activities, making it an excellent option for those who want to increase their overall physical functionality  In this article, let’s look at what functional strength training is, how it can benefit you, and how you can get started with it 
What Is Functional Strength Training?
Functional strength training is a type of exercise that focuses on movements that mimic daily activities and improve an individual&#8217;s ability to perform them with ease  It is different from traditional weightlifting or bodybuilding exercises that focus on specific muscle groups in isolation')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('8f899036-73a1-41ec-a0c8-8d9ed2e3f1ef', '39612469-851d-456c-bf74-d0627821e490', 'Improving Recovery with PNF Techniques', 'Pilates','Pilates', 95215, 283309,'2024-12-04','2024-12-09',11,4,4, 'Congratulations on your new baby! As a new mother, it&#8217;s normal to have questions about when to start exercising after pregnancy and what exercises are safe and effective  This article will guide you through the best post-pregnancy exercises and tips for getting fit 
Benefits of Exercise For New Mothers

Post-pregnancy exercise is a safe and effective way to improve physical and mental health after pregnancy  With the right guidance and support, new mothers can safely and effectively incorporate exercise into their daily routine, and experience the many benefits it has to offer 
Strength and Well-Being:
Exercising after pregnancy is an excellent way to help new mothers regain their physical strength and improve their overall health and well-being')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('10ef2fb2-586d-44a1-b1cd-5d5a47eeeec9', '39612469-851d-456c-bf74-d0627821e490', 'Post Pregnancy Exercise: Benefits and Common Mistakes To Avoid', 'Pilates','Pilates', 390195, 1960482,'2023-04-18','2024-12-11',11,9,3, 'Are you looking for an effective way to improve your muscle recovery after a workout? PNF techniques may be just what you need  In this article, we will explain what PNF techniques are and how they can help you improve your muscle recovery and overall fitness 
Introduction to PNF


PNF techniques are a set of stretching and training methods that can help improve muscle flexibility, strength, and endurance  These techniques are widely used by fitness professionals and athletes to improve their physical performance 
The primary goal of PNF techniques is to stimulate the neuromuscular system to improve the way muscles function')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('54ca8dbb-766f-4ac9-bc81-f4661ee05105', '39612469-851d-456c-bf74-d0627821e490', 'The Beginners Guide to Unilateral Exercises', 'Cardio', 'Cardio', 1624400, 647362,'2023-12-16','2024-12-05',5,2,4, 'If you&#8217;re looking to get fit and improve your overall health, adding unilateral exercises to your workout routine can be an excellent way to achieve your goals  Unilateral exercises involve working one limb at a time, and they offer numerous benefits, including correcting muscle imbalances, improving core stabilization, reducing injury risk, and developing motor skills 
In this article, we&#8217;ll explain what unilateral exercises are, explore their benefits, and show you how to create an exercise plan that incorporates them  We&#8217;ll also provide some examples of the best unilateral exercises for every body part, and answer some frequently asked questions to help you get started 
What Are Unilateral Exercises?
Unilateral exercises are movements that involve working one limb at a time, rather than both limbs simultaneously')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('26c1d561-ee7f-40b1-b472-b49cda2b2e2b', '39612469-851d-456c-bf74-d0627821e490', 'Understanding the General Adaptation Syndrome and Getting Fit Safely', 'Stretching','Stretching', 1740187, 1513643,'2023-04-28','2024-12-20',45,9,2, 'Getting started with exercise can be daunting, especially if you&#8217;re new to it  You may wonder why you need to go through the pain and difficulty of working out  But exercise has a purpose and justification behind it, which we&#8217;ll explain in this article  We&#8217;ll also show you how to get fit safely, even if you&#8217;re a beginner with little knowledge of fitness, exercise science, and nutrition 
The Importance of Exercise for Your Health
Exercise is essential for good health, both physical and mental')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ceb61570-68ff-46b1-91b5-2753da334058', '39612469-851d-456c-bf74-d0627821e490', 'Dumbbell vs Barbell Bench Press: Whats Better for Building Upper Body Strength?', 'Cross fit','Cross fit', 458131, 1518235,'2023-05-11','2024-12-11',15,10,3, 'If you&#8217;re looking to build upper body strength and muscle mass, the bench press is a great exercise to add to your routine  It&#8217;s a popular compound exercise that targets the muscles of the chest, shoulders, and triceps, and can be performed using either a barbell or dumbbells 
In this beginner&#8217;s guide, we&#8217;ll explore the differences between dumbbell bench press and barbell bench press and which one is best for your training goals  We&#8217;ll also cover safety tips, warm-up and cooldown exercises, proper technique, progression and variation, and the role of nutrition in supporting strength training and muscle growth 
What Is The Bench Press?
The bench press is a strength training exercise that primarily targets the muscles of the chest, shoulders, and triceps')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d0f239f5-0d23-44b6-b5a7-74d6595d24d7', '39612469-851d-456c-bf74-d0627821e490', 'Bulgarian Split Squat - The Ultimate Workout Guide ', 'Pilates','Pilates', 1599209, 1673658,'2024-06-23','2024-12-16',54,8,2, 'If you are looking for an effective lower body exercise, then the Bulgarian Split Squat (BSS) might be one of the perfect choices for you  This unilateral movement targets the muscles of the lower body, including the hip flexors, quadriceps, hamstrings, and glutes  In this article, we will discuss the benefits of the BSS, how to perform it correctly, common mistakes to avoid, and different variations of the exercise 
What is the Bulgarian Split Squat?


The Bulgarian Split Squat (BSS) is a unilateral lower body exercise that involves lunging forward with one leg while keeping the other leg behind on a raised surface such as a bench or step 
It is called the Bulgarian Split Squat because it is believed to have originated in Bulgaria, where it was used by weightlifters to build lower body strength and explosiveness')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e2afa08d-1cdc-43e2-ad83-582356a4f5da', '39612469-851d-456c-bf74-d0627821e490', 'Genetics And Athletic Performance: What’s The Connection?', 'Pilates','Pilates', 1872440, 207181,'2023-09-02','2024-12-01',16,6,5, 'Do you love playing sports or working out but feel like you&#8217;re not seeing the results you want? You might be blaming yourself for not working hard enough, but the real culprit could be your genetics  In this article, we&#8217;ll explore how your genetics can affect your athletic performance and what you can do about it 
What do we mean by genetics?
When we talk about genetics in sports, we&#8217;re referring to the physical and physiological attributes that are inherited from our parents and family members  These attributes can include things like: Height Muscle fiber type distribution Limb length Muscle insertions Baseline muscle mass Flexibility Mobility Elaborate on each of these attributes and how genetics can play a role

Height: Height is largely determined by genetics, and it can have an impact on an athlete&#8217;s performance in certain sports  For example, taller athletes may have an advantage in basketball because they can reach higher for rebounds and shots')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('c201e7dc-8b69-4bc1-8fc2-2d588f2b2829', '39612469-851d-456c-bf74-d0627821e490', 'Rationale Behind Stretching Exercises: Benefits and Principles', 'Cardio', 'Cardio', 20222, 493920,'2023-04-18','2024-12-15',28,6,4, 'Stretching exercises are an essential aspect of any fitness regime  The primary objective of stretching exercises is to achieve appropriate muscle length and active range of motion at the respective joints  In this article, we will discuss the principles of stretching exercises, types of stretching, and the benefits of these stretching exercises 
Principles of Stretching
Stretching helps to improve static-passive flexibility and muscle length, reduce muscle imbalances, and improve faulty movement patterns  Faulty movement patterns arise due to altered reciprocal inhibition, wherein the tight agonists (primary mover muscle group) reduce the neural drive of its functional antagonist, and synergist dominance, wherein the muscles that are supposed to assist the primary movers during an activity take over the role because the agonist muscles aren’t activated effectively')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('712a97a8-6fa1-4112-a3bb-f5faff06a27b', '39612469-851d-456c-bf74-d0627821e490', 'Muscle Memory: Understanding the Concept and its Benefits', 'Yoga', 'Yoga', 33296, 1855303,'2023-09-13','2024-12-03',13,6,3, 'Muscle memory is a phenomenon that occurs when a person regains muscle size, strength, or skill quickly even after he or she goes through a period of time without training  In this article, we&#8217;ll discuss what is muscle memory, how it works, and the benefits it offers  We&#8217;ll also share tips on how to improve muscle memory and examples of muscle memory in everyday life 
What is Muscle Memory?


Muscle memory refers to the brain&#8217;s ability to store motor skill information, which allows a person to perform a movement better and with less effort over time  This means that with repetitive movement, the brain retains the information and adapts to it, resulting in neural adaptation, improved strength, and power')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('4fe4f88b-1a1c-4655-9196-8d5b0ddba084', '39612469-851d-456c-bf74-d0627821e490', 'Can You Gain Muscle in Calorie Deficit?: Understand Body Recomposition', 'Cross fit','Cross fit', 287810, 1580041,'2023-09-23','2024-11-25',18,3,3, 'Regular physical activity is important for people of all ages, but it becomes especially important as we get older  Engaging in regular exercise can help seniors maintain their mobility, independence, and overall health  However, many older adults face unique challenges when it comes to exercise, such as chronic health conditions, mobility issues, and a lack of motivation  In this article, we&#8217;ll explore the benefits of exercise for older adults, guidelines for safe and effective exercise, and tips for staying motivated 
Why Should Older Adults Exercise?
Regular physical activity and exercise are beneficial for chronic illness management as well as healthy ageing')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('11d7a6bc-03fb-45ff-ae1e-66fd52cd95cf', '39612469-851d-456c-bf74-d0627821e490', 'Exercise for Elderly: Tips and Guidelines', 'Pilates','Pilates', 1974389, 561905,'2024-11-15','2024-12-02',22,10,2, 'Exercise is a crucial element in weight management  While diet plays an essential role in weight maintenance, exercise can significantly contribute to achieving weight management goals  This article will discuss the role of exercise in weight loss and weight gain, its benefits, tips for incorporating exercise regularly  Don’t miss the detailed FAQs for a better understanding 
The Role of Exercise In Health &amp; Fitness
As any seasoned exercise expert will tell you, exercise plays a crucial role in weight management')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('7d49b5ff-8803-4f4d-aff7-783d6d77b1f4', '39612469-851d-456c-bf74-d0627821e490', 'Exercise For Weight Management And Health: Tips For Getting Started', 'Stretching','Stretching', 1184201, 1210687,'2023-10-18','2024-12-01',1,8,1, 'Many people have two major goals when it comes to fitness: losing fat and gaining muscle  However, it&#8217;s often believed that building strength requires being in a caloric surplus  But is it possible to build muscle while in a calorie deficit? The short answer is yes, but it depends on several factors 
Body Recomposition and Adaptations
There are two ways to increase strength: body recomposition and adaptations  Body recomposition involves losing fat and gaining muscle simultaneously, while adaptations occur through lifting weights and proper recovery')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('34280e59-78a8-4382-994e-844768530174', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Fat Loss vs Weight Loss 101: The Ultimate Beginner’s Guide', 'Strength training','Strength training', 513833, 1346104,'2024-05-03','2024-12-09',39,5,1, 'Are you trying to lose weight but don&#8217;t understand the difference between weight loss and fat loss? Losing weight is a common goal for many people, but it&#8217;s essential to understand the difference between weight loss and fat loss to achieve your desired goals  In this article, we will discuss the difference between weight loss and fat loss and provide tips on how to maximise fat loss while minimising muscle loss 
Understanding the Difference between Weight Loss and Fat Loss
Weight loss and fat loss are often used interchangeably, but they are not the same thing  Weight loss refers to a decrease in overall body weight, which can result from losing body water, bone mass, and even food weight  On the other hand, fat loss specifically refers to losing body fat')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e995b209-cff3-4f42-be24-dfb58b8ab91d', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Cardios Weight Training: Find the Right Balance for Optimal Fitness', 'Yoga', 'Yoga', 1647574, 1596312,'2024-05-03','2024-11-26',34,1,0, 'Are you trying to figure out the best way to achieve your fitness goals? Are you torn between doing cardio or weight training? It&#8217;s a common dilemma that many people face, and it can be overwhelming to decide which one to focus on  In this article, we will explore the benefits of both cardio and weight training, debunk common myths surrounding these exercises, and provide tips on how to balance the two for optimal fitness  We&#8217;ll also discuss how to lose belly fat with a combination of cardio and weight training and answer some frequently asked questions about the topic  By the end of this article, you&#8217;ll have a better understanding of which exercise is right for you and how to incorporate both into your fitness routine 
Understanding Cardio and Weight Training
What Is Cardio?
Cardio, short for cardiovascular exercise, is any activity that raises your heart rate and breathing rate')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b54ebea7-d2af-4811-b337-77fdf7e29ddf', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Push Ups for Beginners: A Guide to Mastering the Basics', 'Pilates','Pilates', 962825, 1429063,'2023-03-08','2024-12-03',13,10,2, 'Periodization training is an essential aspect of designing a training program for athletes and fitness enthusiasts  It involves systematic manipulation of training variables such as volume, intensity, and frequency to achieve specific performance goals  Over the years, two dominant models of periodization have emerged: Linear Periodization (LP) and Non-Linear/Undulating Periodization  In this article, we will delve into these models and discuss their implementation 
What is Periodization?
Periodization is a systematic approach to training that involves the manipulation of training variables such as volume, intensity, and frequency to achieve specific performance goals')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('a97c7c4f-4891-4569-874e-93141d337aca', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Types of Periodization Models in Sports Training: Linear and Non-Linear', 'Strength training','Strength training', 1951180, 223412,'2024-06-16','2024-12-16',10,5,5, 'Push-ups are a fantastic exercise that requires no equipment and can be done anywhere, making it an ideal workout for people who want to improve their upper body strength  For beginners, push-ups can be a challenging exercise, but with the right technique and progression, anyone can learn how to do them correctly  In this article, we will explore different types of push-ups and provide a beginner push-up routine to help you get started 
Which muscles are targeted by Push-ups?
Push-ups are a compound exercise that targets multiple muscles, including the chest, shoulders, triceps, and core  When done correctly, push-ups also engage the muscles in the back and legs to maintain proper form and balance')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('4d4767f8-0e17-4592-a1c5-fb0461a28f33', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Deload Week and Tapering: How To Recover and Improve Sports Performance', 'Stretching','Stretching', 1818711, 121544,'2024-08-11','2024-11-25',35,4,1, 'As athletes, we often push our bodies to the limit to develop performance adaptations such as strength, power, and endurance capacity  However, intense training can lead to body fatigue, which can significantly affect an athlete&#8217;s performance  Deloading is a technique used by athletes to recover from prior intense training periods or blocks in a periodized training plan  In this article, we will discuss deload weeks, tapering, and their benefits in sports training 

What Is a Deload?
Deload is a training technique used by athletes to recover from an intense training period or block')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('2bb5f8d7-59ef-4d1f-aa36-bb3089c83bd8', '609f4589-580b-4810-9518-b3b96ba2cb44', 'What is a Spotter in the Gym? Ground Rules for Effective Gym Spotting', 'Strength training','Strength training', 834844, 1975450,'2024-02-08','2024-11-25',59,10,4, 'A spotter in the gym is an experienced individual who assists athletes in lifting or pushing more weight while ensuring their safety and proper technique  Spotting is crucial for weightlifting exercises, such as bench press, squatting, and deadlifts, as it helps the athlete perform the exercise with more confidence, knowing they have assistance if needed 
What is Gym Spotting?


Gym spotting is the act of assisting a weightlifter with their lifting exercise by standing behind them, helping them with the struggle portion of the lift, and ensuring their safety during the execution  The goal of spotting is to help the athlete complete the lift, ensure proper technique, and prevent injury 
Why do you need a spotter in the Gym?
Having a spotter in the gym is crucial for weightlifting exercises that involve heavy weights')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('16286a50-8b01-47ef-b0d0-46a46bdf522b', '609f4589-580b-4810-9518-b3b96ba2cb44', '​​Understanding Strength: Absolute vs Relative Strength in Fitness', 'Cross fit','Cross fit', 1516130, 258933,'2023-04-06','2024-12-19',20,3,2, 'Parallel bar dips, also known as parallel dips or parallel bar triceps dips, are a compound exercise that targets the chest, shoulders, and triceps muscles  They are an excellent addition to any upper body push day, and they can be used to train for muscular hypertrophy, upper body strength, and/or endurance, depending upon the load used 
In this article, we will discuss the correct way to perform parallel bar dips, the benefits of the exercise, and some variations to make it easier or more challenging  We will also provide FAQs based on the article to serve as a ready reckoner for people 
Performing Parallel Bar Dips: Correct Technique &amp; Form
To perform parallel bar dips, one needs access to a dip stand or parallel bars')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('2d98ac9c-c78e-4c1a-93ff-bdd4438193dd', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Parallel Bar Dips: An Underrated Upper Body Exercise', 'Yoga', 'Yoga', 1754075, 1783874,'2023-02-11','2024-12-01',9,6,2, 'Strength is a crucial element in physical fitness that helps improve overall health, sports performance, and everyday activities  However, measuring strength can be complex as there are various factors to consider, including body weight, height, and the specific exercise being performed  In this article, we will delve into two different ways to look at strength &#8211; absolute and relative strength &#8211; and the various factors that influence these measures  We will also provide information on strength standards for women and men, as well as answer some frequently asked questions about measuring and improving strength  Whether you&#8217;re a beginner or an experienced athlete, this article will help you better understand your strength and how to improve it')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('02b6d198-aa5d-4439-85ef-62ecbf67edf6', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Sumo Deadlift 101: Understanding the Sumo Deadlift and Its Benefits', 'Stretching','Stretching', 154924, 1214360,'2024-04-28','2024-12-05',24,6,5, '&nbsp;
When it comes to building muscle mass, many of you find yourselves at a crossroads  You can’t decide if compound exercises are best for hypertrophy or isolation exercises  In this article, we will break down the differences between these two types of exercises and determine which one is better for hypertrophy 
Compound vs Isolation Exercises: What&#8217;s the Difference?

Compound exercises are movements that involve multiple joints and work multiple muscle groups simultaneously  Examples of compound exercises include squats, deadlifts, and bench presses')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('7471b4ba-153f-4963-be74-ecb0707ecf2a', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Compound vs Isolation Exercises: Which One is Best for Hypertrophy?', 'Pilates','Pilates', 1878301, 1472765,'2023-10-28','2024-11-25',24,3,0, 'Are you looking to lose fat and get fit? Resistance training may be the solution you&#8217;ve been searching for! This type of exercise involves using weights or resistance bands to build muscle and burn fat  In this article, we&#8217;ll explain the science behind resistance training for fat loss, compare resistance training vs cardio for fat loss and provide practical tips and advice for beginners who want to start a resistance training program 
The Science Behind Fat Loss
When it comes to fat loss, many people turn to cardio exercise as their go-to strategy  While cardio can certainly be effective for burning calories and shedding pounds, it&#8217;s not the only option  Resistance training is a powerful tool for fat loss, as it can increase muscle mass, which in turn boosts metabolism and leads to long-term fat burning')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ade4ddd0-943c-43b1-825a-76414ed75d3d', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Resistance Training for Fat Loss: Build Muscle and Boost Metabolism', 'Strength training','Strength training', 1938597, 399970,'2024-11-26','2024-11-29',52,6,2, 'Are you trying to lose weight but don&#8217;t understand the difference between weight loss and fat loss? Losing weight is a common goal for many people, but it&#8217;s essential to understand the difference between weight loss and fat loss to achieve your desired goals  In this article, we will discuss the difference between weight loss and fat loss and provide tips on how to maximise fat loss while minimising muscle loss 
Understanding the Difference between Weight Loss and Fat Loss
Weight loss and fat loss are often used interchangeably, but they are not the same thing  Weight loss refers to a decrease in overall body weight, which can result from losing body water, bone mass, and even food weight  On the other hand, fat loss specifically refers to losing body fat')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('6b1e7876-493d-47b2-a7f7-4b0bc10f037d', '609f4589-580b-4810-9518-b3b96ba2cb44', 'Fat Loss vs Weight Loss 101: The Ultimate Beginner’s Guide', 'Cardio', 'Cardio', 1721744, 937657,'2023-02-06','2024-12-20',30,5,4, 'Sumo Deadlift: What is it and how to perform it correctly?


If you&#8217;re looking to build total-body strength, the deadlift is a must-have in your workout routine  Deadlifting involves lifting a heavy barbell off the floor from a dead position, using a hip-hinge movement that engages multiple muscle groups  In this article, we&#8217;ll explore the sumo deadlift &#8211; a variation of the traditional deadlift that involves a wider stance and unique biomechanics  We&#8217;ll delve into the muscles worked, form, benefits, and differences between the sumo and conventional deadlifts 
Muscles Worked in the Sumo Deadlift
The sumo deadlift is a compound exercise that targets multiple muscle groups in your body')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('71a749fd-8046-4848-a1ee-2fd44ca3ceff', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Can teenagers perform weight training?', 'Cross fit','Cross fit', 189751, 1725942,'2024-05-19','2024-12-14',46,7,1, 'Weight training is thought to be detrimental to children’s bone health and hence they are discouraged to start training at an early age as it may result in a lesser stature(height) of an individual  The three popular rationales behind suggesting not to train with weight before or during puberty are undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('96d014df-f0af-4b4c-957c-3eb823bdc6ab', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Whats the difference between Conventional and Romanian Deadlift?', 'Strength training','Strength training', 396447, 186541,'2023-02-21','2024-12-11',55,3,0, 'The hip hinge is a crucial movement pattern, so it is critical to find a comfortable variation to perform (if able) and work on it  But when discussing the Conventional deadlift (CDL) and Romanian deadlift (RDL), the latter is an example of pure hip hinge compared to the former  So when a coach is teaching an individual to perform a deadlift variation at the local training facility  How does a third person know which deadlift variation is getting performed? undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('fa3180d6-5fc0-4430-ad3d-ed644ca4dafe', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'What are the benefits of romanian deadlift?', 'Pilates','Pilates', 347348, 505062,'2023-01-27','2024-12-08',45,8,2, 'A compound movement which involves various joints (the shoulders, elbows and wrists)  Being the versatile exercise it is, the bench press can be a great option to include for a powerlifter, bodybuilder and even for someone looking for maintaining general fitness  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('2a36b546-9543-498f-b5ec-96e5922a96f9', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'The right technique to bench press', 'Cross fit','Cross fit', 1499245, 1570875,'2024-09-23','2024-12-03',3,2,0, 'The Romanian deadlift is one of the versatile and effective deadlift variations that target posterior chain muscles, including erector spinae, glutes, and hamstrings without putting a lot of training stress  It begins with the eccentric phase, ensuring the tension on the posterior chain to stimulate muscular growth in the glutes and hamstrings undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('21b77d82-b314-4f9a-b688-015e0c01c4e8', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'The right technique to squat', 'Cross fit','Cross fit', 700905, 1674741,'2023-05-14','2024-12-16',41,3,2, 'The squat is a very basic bodyweight exercise which is great when it comes to hitting most of the muscle groups in your lower body  It targets the quadriceps (the front part of your thigh), glutes, hamstrings, adductors, hip flexors and calves”  Depending on the type of squats (yes, you can choose from a bunch of different types of squats depending on your goal), a few more muscle groups, like the muscles of the six pack (rectus abdominis), obliques, and lower back (erector spinae), often referred to as the ‘core muscles”  It’s a compound exercise, which involves movement of the hip, knee and the ankle joint ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('7a7cf7f5-1f88-4ea3-9986-7d0db7c5f928', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Equipments for exercising at home', 'Strength training','Strength training', 1952703, 483396,'2023-10-29','2024-12-12',8,9,0, 'With the rise of work from home, there’s been a huge demand for home fitness solutions  The virtual training and home workouts have achieved a new cornerstone in personal fitness, even as the gyms slowly begin to welcome back their customers around the country  Were all busy people with our jobs, family, meetings, cooking, etc, and the majority of us are fairly adept at coming up with excuses to avoid going to the gym  However, being active is crucial  Not only can it help you accelerate your fat loss goal if that’s the goal, but it also provides plenty of other health advantages, from strong bones and muscles, and keeping your sanity sane to reducing stress and anxiety')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('cc621e12-4628-46f5-9d33-77420f589659', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'How to increase your bench press?', 'Cross fit','Cross fit', 292001, 729518,'2024-06-19','2024-12-13',15,2,5, 'Bench press is thought to be the king of all exercises targeting pectoral muscles but sometimes people struggle to increase the weight on the bar and may feel stuck  So this article will cover a few of the common reasons why one may be struggling to increase the strength of the Bench Press  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b3ba9924-7cfc-4721-a616-f61585ac4f78', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Does Hydration Impact Performance', 'Cardio', 'Cardio', 1169298, 1751317,'2024-06-20','2024-11-29',20,3,2, 'Hydration is a necessary part of the human bodys process to keep itself cool, and its vital to make sure youre drinking enough water to keep yourself hydrated  Without it, you risk having your performance affected  Even though it is individual-specific but studies do show that even 3% of dehydration can cause a decrease in strength, power, and endurance of the body ( Savoie et al ,2015 ) ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('abac0005-3a3f-4567-9f77-2ef2f46970ee', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Exercising and Fatloss', 'Cross fit','Cross fit', 4494, 486200,'2024-06-02','2024-11-27',53,3,5, 'Weight loss or weight gain is determined by the number of calories a person consumes, and weight or fat loss is determined by the type of activities a person does! undefined undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e4add69d-70e1-4f2e-bfe1-351ce70313c8', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Basic science of muscle-gain', 'Cardio', 'Cardio', 1994890, 1818748,'2024-08-17','2024-11-25',29,7,5, 'In the fitness fraternity, the phrases progression and progressive overload are sometimes used interchangeably  Although both terms refer to the process of progressing training, these are not synonymous  These distinctions have significant consequences for how we think about creating particular fitness traits, as well as approach programming to induce adaptations  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('20237c27-9863-40ba-ac9c-a64e7adc1a64', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Progression and Progressive Overload are not the same thing !', 'Stretching','Stretching', 239855, 39707,'2023-12-22','2024-12-11',2,3,1, 'Theres a lot to learn about hypertrophy but where to begin? When it comes to figuring out how to train for muscular hypertrophy effectively, many individuals mistake the forest for the trees  This article will help in understanding some of the fundamental programming principles  for hypertrophy  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('3da4a5f0-7ef3-4726-a4d2-485b15513bb4', '82f951cc-a6ad-4ee0-b719-184190f4eaed', 'Dealing with Diastaisis Recti', 'Strength training','Strength training', 410264, 1688728,'2023-08-20','2024-12-07',39,6,1, 'Diastasis Recti is a condition in which the rectus abdominus muscles separate but with no fascia defect during pregnancy  A protruding midline is a characteristic of Diastasis Recti, which is caused by an increase in intra-abdominal pressure  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('57f9cbd2-1447-4ed4-be70-5257650ab501', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Dealing with Diastaisis Recti', 'Stretching','Stretching', 1916909, 1685436,'2023-12-25','2024-11-21',17,10,5, '“Before starting a resistance training session one should always warm-up ” This is very common to hear but it is also common to see the warm-up being done in not so optimal way so we will be discussing how to warm up properly  Before we do that, let’s understand the need for a warm-up routine so that the movements in the warm-up routine will make sense  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('5b4c801f-8672-4925-ad85-f0f52e588cd1', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Some Useful Warm-up Suggestions', 'Cross fit','Cross fit', 1440886, 1997750,'2023-06-12','2024-12-14',2,6,1, 'Diastasis Recti is a condition in which the rectus abdominus muscles separate but with no fascia defect during pregnancy  A protruding midline is a characteristic of Diastasis Recti, which is caused by an increase in intra-abdominal pressure  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('751dd016-4b9b-4cfc-b5ab-1b644566d830', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Key exercise guidelines during pregnancy', 'Strength training','Strength training', 1396421, 832503,'2024-05-28','2024-12-20',15,9,4, 'One of the most common worries new moms have is whether or not exercising and dieting will influence their milk production during nursing  How can they start losing weight while breastfeeding? What steps need to be taken to reduce weight while simultaneously ensuring that the child receives the proper nutrition and etc  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e305ff28-576e-472f-a6c6-5fe4c89b3d48', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Is a calorie deficit appropriate when breastfeeding?', 'Cross fit','Cross fit', 1807679, 241116,'2023-04-16','2024-11-23',48,7,0, 'During pregnancy, the body goes through a lot of changes  Some changes may limit the ability to perform different movements or require modification in the workout program  Changes in hormones and the production of the Relaxin hormone can weaken ligaments, and increase the risk of joint damage (such as sprains)  As the pregnancy advances, the center of gravity shifts forward, affecting both balance and coordination  The resting heart rate rises and blood pressure can drop during pregnancy')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('c1c00896-60fa-430e-999f-5d94780014e7', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Lifestyle measures for osteoporosis', 'Yoga', 'Yoga', 221172, 740143,'2023-09-30','2024-12-12',36,2,5, 'Core Training has been one of the strongest buzzwords for the fitness industry in the last ten years  Some of the celebrity coaches took note of peoples obsession with a flat stomach  As a result, they created a business opportunity thats still generating revenue out of innocent gym goers pockets  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('72402f43-0550-4f17-aa70-9640eaaf7434', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Beginners Guide to Core Training - Part 1', 'Strength training','Strength training', 1913561, 200696,'2023-06-13','2024-12-11',11,4,1, 'Osteo means bones, and porosis refers to "holes" or "porous structures " Osteoporosis is a silent health condition  It is a common and chronic metabolic bone disease that weakens the bones, making them porous and fragile, and increased fracture risk  However, it doesnt have symptoms, develops slowly over several years, and is often only diagnosed when a fall or sudden impact causes a bone to break (fracture)  Broken wrists, hip fractures, and spinal fractures are the most common injuries in patients with osteoporosis')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('68afd51e-9c42-4461-bce1-771a455ad167', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Popular Training Mistakes', 'Pilates','Pilates', 1353212, 781905,'2024-07-07','2024-12-13',53,7,3, 'It’s very common to see beginners making some fundamental errors when starting with their training  Things get worse when they don’t get to learn these errors and some of them end up committing these same mistakes for years  This article will discuss some of the most common errors and hopefully help you in correcting the mistakes if you have been doing them  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('4261e197-bd04-4163-ae9f-365d47651209', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Do you know how your body gets energy?', 'Strength training','Strength training', 893061, 1011471,'2023-01-02','2024-12-13',27,6,1, 'What all systems are there inside this machine, which uses the fuels, and in what conditions, they are getting used  Wherever we do a 30 km run, or while I’m typing this line, everything is powered by one and only compound, ATP  But – Why do all the movements look different? Why does running exhaust a person, but walking doesn’t? Why can athletes keep on working out for very long, and some just get exhausted in a very short period of time? undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b4302f9b-7c0f-4c71-b655-c06686e2ec2f', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Why is breathing right important during resistance training?', 'Cross fit','Cross fit', 936185, 480463,'2023-10-05','2024-12-10',28,8,4, 'Taking a breath is the most natural action for an individual; it is needed to sustain life and survive  But when it comes to physical fitness, breathing patterns often ranks last in order of priority  An individual tends to focus more on exercise selection, training intensity, volume, rest periods, etc  Although the mentioned variables are essential, the proper breathwork should form a part of the initial technique and base work ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('37ba7a59-2266-4dad-a1e7-d52deee311b7', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Free weights v/s Machines  - Which is better?', 'Strength training','Strength training', 591026, 1358882,'2024-11-16','2024-11-25',22,5,2, 'Many gyms appear to have different types of equipment  Dumbbells, medicine balls, cable pulleys, ropes, and machines  These are basic options for strength training: undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('363b681a-2016-4da8-9efb-e0b047325dab', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'Ways to save time in the gym', 'Yoga', 'Yoga', 251606, 197000,'2024-11-11','2024-12-16',17,2,1, '“How much weight should we lift?” is one of the most frequently asked questions in the fitness community  The answer to this can’t be answered without knowing the goal of a person so this article will establish recommendations based on the goals  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d51808db-ae01-49c4-97dc-b75db10cdddb', '97eaabf4-bd60-461b-88be-69caa4d9f889', 'How much weight should you lift?', 'Pilates','Pilates', 1662842, 852778,'2024-10-18','2024-11-22',30,3,2, 'If someone has very minimal time to work out or cannot dedicate long hours to training due to non-fitness obligations, work, family, etc  there are plenty of ways that can minimize the training time and provide effective training sessions  Here are certain pointers that can be implemented  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e70f14d1-abf9-48b6-b9a1-7b88cd75a452', '9ff36136-6197-48c7-85d5-7a17660a535b', 'How to train for strength around menstrual cycle?', 'Cardio', 'Cardio', 1651630, 1171113,'2024-07-02','2024-12-15',51,7,2, 'Women go through the menstrual cycle because of which there are some hormonal fluctuations that affect how they feel  Due to these fluctuations, it ultimately reflects on the strength levels  They feel strong on certain days and relatively weaker on other days  This article intends to establish recommendations for training adjustments that will practically suit the best for women and it will also answer some of the common questions around the topic of the menstrual cycle ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('672d5fac-1c7b-43dd-a52b-55c9c7b4c564', '9ff36136-6197-48c7-85d5-7a17660a535b', 'How long should be the rest period between the sets?', 'Yoga', 'Yoga', 1134896, 1704864,'2024-08-01','2024-11-29',12,1,3, 'Calisthenics comes from two Greek words ‘ Kalos’ and ‘ Sthenos’ which means Beauty and Strength respectively  Even though it can be done with just bodyweight and from anywhere with minimal equipment, it is one of the most versatile forms of training  This training form has been there since old times when there were no machines or fancy equipment to work out with  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1750fb51-54ab-49b4-9088-75f50b35a31d', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Whats the difference between beginner, intermediate and advance trainee?', 'Strength training','Strength training', 193553, 1736856,'2024-02-16','2024-12-20',0,7,3, 'It is very common to distinguish a trainee as beginner, intermediate or advanced on the basis of the duration of training  In this method, a trainee can be categorized as follows depending upon the training age undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('9025bd06-7199-4026-b603-750f96724553', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Versatility of Calisthenics', 'Cardio', 'Cardio', 455585, 1557741,'2024-10-12','2024-12-12',11,2,4, 'The rest period denotes the duration of rest taken between the sets of the same or different exercises  The duration of rest that should be taken by an individual to get optimal results depends on the goal  If too long rest is taken then necessary adaptations may not take place and if too little rest is taken then performance may drop  The recommended rest period may differ for endurance, hypertrophy, and strength training ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('fe45080f-94c6-4570-96db-27ad3cdfe782', '9ff36136-6197-48c7-85d5-7a17660a535b', 'What is Mind Muscle Connection?', 'Stretching','Stretching', 1581724, 1749221,'2024-10-18','2024-12-12',42,7,2, 'Mind muscle connection is referred to as the ability of internal cueing of the muscle for better activation or attempt to perform harder muscle contraction by increasing the neurological drive  For ages, various experts believed that the mind-muscle connection is essential for maximizing growth  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e2d12459-d1da-414f-9277-4f6b6327fff1', '9ff36136-6197-48c7-85d5-7a17660a535b', 'How to Improve the Quality of Life?', 'Cardio', 'Cardio', 1804218, 951767,'2023-09-26','2024-12-04',49,5,0, 'If one is looking for a way to improve overall health and wellness, exercise is one of the best ways to do it  Not only does it help in becoming physically better, but it can have a positive impact on emotional and psychological wellbeing  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('7ea09a72-fda9-45c1-a82e-c706e509f360', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Pregnancy tips for the fourth trimester', 'Yoga', 'Yoga', 1231526, 1141819,'2024-05-15','2024-12-10',57,3,2, 'A woman has spent days, weeks, months, even years, mentally and physically preparing herself for this day  The baby has arrived    So, whats next? Everything is about to change!')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('9ab2c8d7-caaa-4e08-8304-81e1f83751fd', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Beginners Guide to Core Training - Part 3', 'Cardio', 'Cardio', 731638, 42319,'2023-12-27','2024-12-04',46,3,1, 'To understand more about core training via other exercises such as isometric, dynamic and resistance training, continue reading  In the last part of the series on beginners guide to core training, lets look at how movements such as isometric, dynamic and resistance exercises play a role in training the core  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e750bd85-2e87-4ac4-a000-cc228973b885', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Beginners Guide to Core Training - Part 4', 'Strength training','Strength training', 414102, 1376050,'2024-05-09','2024-11-23',42,10,2, 'Anti-rotation exercises are the ones that teach the core to resist rotation places via lateral resistance forces  A stable body is necessary to efficiently transfer forces between the upper and lower limbs  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('4946c22a-08e2-4d12-8387-43f7d0de55b5', '9ff36136-6197-48c7-85d5-7a17660a535b', 'What is Velocity-Based Training (VBT)?', 'Yoga', 'Yoga', 1871908, 315932,'2024-03-13','2024-12-05',35,4,0, 'Resistance training is an effective way of developing strength, power, and muscular endurance  But, depending upon the athletes goal, coaches manipulate various resistance training variables like exercise selection, exercise order, frequency, intensity, volume, and rest periods for specific physiological and neuromuscular adaptations  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('fc995911-7ae2-4647-9641-7b64c1479f24', '9ff36136-6197-48c7-85d5-7a17660a535b', 'Beginners Guide to Core Training - Part 2', 'Yoga', 'Yoga', 531142, 753559,'2023-01-18','2024-12-04',50,1,4, 'In strength training it is very important to strategize the sequence of exercises in such a way that a person makes the best use of the effort in a session  Inefficient order of exercises can hamper the overall performance and ultimately affect the results  The ordering of exercises can be done on the basis of the below-mentioned points undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ed1773ed-6084-4b72-9fab-1b9dfb58e850', '9ff36136-6197-48c7-85d5-7a17660a535b', 'The best way to order exercises for optimal results', 'Yoga', 'Yoga', 954218, 1809597,'2023-05-02','2024-11-30',20,1,5, 'Bracing: Bracing plays a critical role in training, mainly to provide spinal stability  Additionally, its an effective way of training the local core musculature, strongly advocated when teaching functional movements, especially the squat, deadlift and overhead exercises  For example, strict press, push press and push jerk place high demand for a stable spine under heavy loads  An efficient breathing technique often accompanies bracing  Taking a deep breath fills the belly with air; clenching the abdomen creates a rigid midsection')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('43ef809a-b2dc-47cc-ade9-79f47fb50624', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Myths about training', 'Cardio', 'Cardio', 1318836, 1960024,'2023-12-06','2024-12-08',38,4,3, 'Osteoarthritis (O A ) is a common joint condition affecting older people, approximately 10% to 20% of people ≥60 years old worldwide  It is a painful and debilitating joint disease and is a leading cause of disability  As the pain becomes chronic, it may occur at rest and during the night')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1da278e1-85ec-4a5a-b3b2-5a476c62e8ab', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Exercises in Osteoarthritis', 'Yoga', 'Yoga', 1300788, 990185,'2023-03-26','2024-11-24',20,3,5, 'When youre looking for advice on fitness, it can be tough to know what to believe and what to ignore  A lot of the information out there is contradictory, and that can make it difficult to tell what actually works  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('9cc15318-d5b6-4ef9-b607-1fc34d0cef2c', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'How to use Periodization in Resistance Training?', 'Cross fit','Cross fit', 1246261, 372675,'2023-11-12','2024-11-25',39,4,1, 'Physical activity is an essential part of a healthy pregnancy __ that has been shown to benefit the majority of women  The Centers for Disease Control and Prevention  and the American College of Obstetricians and Gynecologists recommend that pregnant women should get at least 150 mins of moderate-intensity physical activity per week i e 30 mins per day  There are numerous benefits of physical activity during pregnancy, despite that pregnant women fail to achieve the recommended amount of physical activity during this crucial time ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('8c8ba9a3-b839-4c5e-be3d-bcc79e8c1fcd', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Barriers to exercise during pregnancy', 'Yoga', 'Yoga', 1589081, 1048055,'2024-06-02','2024-12-02',35,7,4, 'A typical day at school consists of approximately 6 hours  During the day, there are brief periods devoted to sharpening the kids different skill sets during the day, such as drawing class, science period, math class, art, craft session, physical activity period, etc  Similarly, Periodization in training divides time into specific periods dedicated to enhancing an individuals particular physical and neurological traits such as mobility, stability, strength, speed, power, agility, quickness, etc  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('a6a3fad5-9979-43d1-a51d-6784146d678d', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Whats the difference between Training and Running Shoes?', 'Cardio', 'Cardio', 1674609, 1865826,'2024-11-30','2024-12-14',41,3,0, 'To perform well in running and resistance training, an individual needs adequate mobility, flexibility, and stability  The foot sets the foundation for stability in movement, and a network of four different joints supports it  In addition, the human foot has a natural arch that helps correct weight distribution among the foot tripod  The foot tripod consists of three vital points: the ball of the big toe, the base of the small toe, and the center of the heel  Thus, the right pair of training shoes are needed to support the natural arch system and load distribution of the body weight either during the run or resistance training')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('33617dcf-77f7-417e-9265-f79558d4d7e8', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Tips for training with an injury', 'Cardio', 'Cardio', 260319, 1682941,'2024-04-15','2024-12-18',55,4,0, 'When it comes to exercise, injuries are an unfortunate reality and get in the way of training for someone who is into proper training  No pain, no gain is the biggest lie in this fitness industry which does more harm than good  When someone experiences injuries, avoid pushing it through  The main goal should be to work around it  Exercising through discomfort/ pain just makes matters worse')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('c3051fcf-9ae8-42dc-8ebb-735cad1a6ac3', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'What is the importance of core stability?', 'Pilates','Pilates', 1224869, 1153895,'2024-03-17','2024-11-28',36,3,1, 'Core stability is an often used term by the fitness community  Certain exercises, which are claimed to be more “functional” usually have a core component in them  What exactly is the “Core” and why stabilizing it is considered so important ? What are the exercises we can use to train our core effectively ? Well, read on  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('33c1d90c-73ed-4b13-bf1b-b4a3f2aab1f4', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'How to squat without a squat rack ?', 'Cardio', 'Cardio', 1311853, 1501237,'2024-04-07','2024-12-04',58,2,4, 'The Squat rack is a piece of equipment, most commonly seen in gyms all around the world  It helps one perform barbell back or front squats without having to lift the bar from the ground, which can be difficult at times, especially if the load is heavy  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('2b1de040-0bf5-4862-a473-7b324268a136', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'How to grow Deltoid Muscle?', 'Cross fit','Cross fit', 1875257, 1994377,'2024-02-26','2024-12-02',21,3,5, 'Deltoid is a muscle group that sits on the shoulder joint and it is commonly referred to as shoulder muscles  Having a well-built deltoid muscle gives an amazing shape to the upper body and it also assists in many upper body exercises  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('16d4e9e9-a8e1-4c47-99ef-c77936a7c284', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Functional training : Just fancy or important?', 'Strength training','Strength training', 1758873, 569186,'2024-03-11','2024-11-23',59,7,2, 'While strength training itself could have a huge positive carryover in dealing with these side-effects of aging, the inclusion of Balance training could help improve posture control, reduce the falling rate and overall performance in an individual(Zech et al , 2010; Gillespie et al  , 2012; Lesinski et al  , 2015) undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('624e128d-4bd7-449d-b13a-256c84c2c1b1', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Why is Balance Training Important?', 'Yoga', 'Yoga', 837299, 991117,'2024-06-12','2024-12-03',50,2,3, 'When I was new to Fitness, I had a perception that functional training is a cool but tough form of training that only experts could do  As I spent years in the industry, I learned and realized that functional training is nothing but training in movements that translates to real-life movements  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('842e4d32-955e-418b-a8cf-95ee8b7a60f5', 'abf5684e-1fc5-479d-a093-d4716b5928b7', 'Simplifying exercise programming using Movement Patterns.', 'Cross fit','Cross fit', 1937535, 650040,'2023-08-26','2024-11-23',27,4,3, 'A few thousand years ago, humankind did not have a squat rack or a deadlift platform  Neither they had a seated row nor a chest press machine  Lastly, not even a single pair of dumbbells or resistance bands  But, humans were still using the same movements back then as the 8-times Mr Olympia Ronnie Coleman did back in the gym  Of course, there could be a newbie who may not know Ronnie Coleman')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('161f53b2-8341-4e41-be29-0a3f4ead92de', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Weight training versus Weightlifting.', 'Cardio', 'Cardio', 225853, 230093,'2024-01-09','2024-11-26',41,10,1, 'Resistance training is a form of physical training in which an individual performs specific movements to challenge the musculoskeletal system  These movements are sometimes also referred to as primary functional movements  The exercises use the forces of gravity to create resistance by using various tools such as barbells, dumbbells, weight stacks, and plate-loaded machines  For example, when choosing free weights, an individual can target specific muscle groups to drive particular adaptations in the musculature by varying the critical training variables such as frequency, intensity, and volume, among others  Additionally, other forms of resistance training include resistance bands, tubes, tires, battle ropes, etc')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('a21f350a-bfdd-4cbd-b3a6-98b793a2a452', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'All about how to foam roll', 'Pilates','Pilates', 1015683, 1291311,'2023-08-21','2024-12-05',28,4,5, 'Plyometric, aka jump training, is a popular concept among fitness enthusiasts, clinicians, and strength and conditioning coaches alike  It refers to exercises that involve rapid stretching and contracting (jumping and rebounding) of muscles to increase power output  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('66222930-e770-4995-af55-bfa5a999afdf', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'How to achieve optimal performance through training?', 'Strength training','Strength training', 1546668, 1611855,'2024-04-22','2024-12-05',31,8,2, 'Training attributes such as endurance, hypertrophy, strength, and power are essential to optimal human performance  Therefore, individuals practice these characteristics based on the training objective laid by the coach  An effective training program will focus on all training attributes for different degrees of time length, depending on the need analysis of the trainee  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('18f1756e-ca15-48ed-a8c1-d000b0308ec0', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'How to initiate plyometric training?', 'Cardio', 'Cardio', 288749, 1618994,'2024-11-13','2024-12-08',35,3,4, 'Flexibility is simply the total range of motion (ROM) available at a joint  And ROM is regulated by the extensibility of the soft tissues like the tendon and ligaments surrounding an individuals joint  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('06dcba2a-5171-4e22-8462-82d7bf8504f2', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Pelvic Health Consideration during Pregnancy', 'Stretching','Stretching', 1815972, 1273816,'2023-08-13','2024-12-10',36,4,3, 'Many individuals desire a strong core for a variety of reasons, including function and aesthetics but they prioritize only crunches to target the abdominal muscles  The abdominal muscles are superficial and give us the six-pack look, but it is actually more complex than that  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1ba1559c-9b4c-4945-90d7-dda07de89558', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'How to build a strong core?', 'Stretching','Stretching', 1134486, 1637446,'2024-09-08','2024-11-22',56,2,2, 'A ** broad ‘sheet’ of muscle rather than a sphincter  It is divided into superficial and deep layers  The superficial layer is found around the opening of the vagina, while the deep layer fans out under the pelvic organs and makes up the vaginal walls  The bladder, uterus (womb), and intestines are all supported by the pelvic floor, which is made up of muscles and ligaments  The urethra, which comes from the bladder, the vagina, which comes from the uterus, and the anus, which comes from the intestine, all pass through the pelvic floor')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('71b1d49e-5671-43b9-80fd-546f3ab11c8d', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Ways to overcome muscle soreness', 'Strength training','Strength training', 615069, 134434,'2023-05-04','2024-11-28',40,9,4, 'Many people have been working from home for several months, which forces them to sit for long periods of time  Sitting for long periods of time is the major reason for tight hip flexors  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('0f6d52bf-2d62-4b52-b1a5-9b35d67faf01', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Improving hip joint mobility', 'Stretching','Stretching', 804721, 1204516,'2023-11-02','2024-11-21',40,1,1, 'If someone has just started working out, introduced a new sport or workout routine,  danced like crazy at a friend’s wedding, increased the workout duration, or had an intense workout session they might experience some muscular pain  undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('596c5c73-4467-4d4f-876f-d17e7811c51c', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Exercise tips during pregnancy', 'Strength training','Strength training', 1078141, 1821963,'2023-01-28','2024-12-02',37,9,3, 'Most women are concerned about exercise during pregnancy  Physical activity and exercise in the absence of any contraindication during pregnancy are safe and have been shown to benefit __ most women  Studies have found that women who exercise during pregnancy have a lower incidence of gestational diabetes, cesarean birth, excessive weight gain, pre-term birth, and higher incidence of vaginal birth, faster recovery post-delivery  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('0f52f5ef-689e-4e97-9c52-68b5e852226d', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Exercising while Breastfeeding', 'Strength training','Strength training', 657076, 842619,'2023-01-28','2024-12-07',53,9,1, 'Exercise is very crucial for breastfeeding mothers  Regular exercise has been shown to reduce stress and assist with treating post-natal depression  It aids in mental well-being, and stress reduction, and functions better in day-to-day life post-delivery  New mothers might find it challenging to get back to their fitness journey but having a baby shouldn’t stop but rather motivate them to pursue their fitness journey  But there can be certain questions or doubts regarding exercise while breastfeeding')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('6471fa94-9112-41b5-af4e-8438475ab06f', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'Bodyweight exercise to target arms', 'Yoga', 'Yoga', 1949855, 1256765,'2023-02-09','2024-11-25',41,5,4, 'Ladies want to tone their arms; Men want to grow them bigger  Almost anybody who works out with weights has done a bicep curl in their life  They look good and are the first thing people notice when you tell them (or don’t) that you have started hitting the gym  Well, what if we tell you that you don’t need an expensive gym membership to train those guns and horseshoe triceps ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('605ffde1-043c-427d-acd4-1c770b7b596f', 'ad96d9cf-d238-4fb7-a63b-27dbe3c54914', 'What are the benefits of jump squats?', 'Yoga', 'Yoga', 364965, 1868068,'2024-04-20','2024-11-22',16,7,3, 'A Squat Jump is just what the name says, a squat with a jump  Its a bodyweight plyometric exercise  Plyometric refers to a  set of exercises that enable a muscle to reach its maximum force potential in the shortest time period  Its a movement performed using quick eccentrics followed by an explosive concentric  to develop  quickness in the lower body ')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1bb56cff-df98-471f-bc6c-4bf09b2dbde7', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'Front squat or Back squat,which one is right for you?', 'Yoga', 'Yoga', 1661204, 960428,'2023-01-28','2024-12-15',7,7,4, 'One should have the ability to perform both but the requirement of one over the other is dependent on the individuals goal  For example, the front squat should be the prime focus if the goal is Olympic lifting and the back squat should be the prime focus if the goal is powerlifting  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d55bd31b-57bb-4186-b4a0-55a84f772e17', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'Should a runner train with weights?', 'Strength training','Strength training', 56857, 1563301,'2024-06-25','2024-12-09',8,9,4, 'Before I get into what to do as an athlete, I would like to highlight the areas of an athlete; target sport, ideal body composition, optimal health, focused mindset, and proper recovery  undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('53cf806d-c778-4aa0-8c9d-018920a97148', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'Glute Training: Aesthetics or Function?', 'Cross fit','Cross fit', 1036340, 1142430,'2023-11-21','2024-12-02',19,10,2, 'It is not uncommon to see social media influencers being known for well-defined glutes  A lot of us might want to build glutes that are attractive  Now the question here is: Is Glutes only for aesthetics or it has any important function too? undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('a4a28e63-05d9-4987-b7e5-36350c474a81', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', '5 tips to keep in mind while training like an athlete from Home', 'Stretching','Stretching', 439980, 1017794,'2024-10-04','2024-12-21',51,3,1, 'Long-distance running heavily relies on our cardiovascular endurance, ability to sustain energy for longer duration and more than speed, it requires endurance  Practicing more medium and long-distance running while using modalities like cycling or swimming on a few days could help get better at the sport and minimize the risk of injury  undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('0b89d622-63eb-448e-898e-1b531f4cec3b', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'How can you improve your Strength?', 'Yoga', 'Yoga', 1075776, 1433427,'2024-02-17','2024-11-29',0,3,3, 'Did you ever take a break, either short or long from your regular training? And wondered if you lost those gains or lost the strength to lift your last personal record? undefined undefined undefined undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('fb15c268-c397-4794-ad39-22a2e592a7c1', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'How to return to training after a break?', 'Cardio', 'Cardio', 1843845, 1037344,'2024-03-15','2024-12-11',38,7,0, 'Apart from big muscles, Strength is one thing that people like to flex  A lot of people believe that if people have bigger muscles, they would be strong too  Well, this is only partially true  Surprised? undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('8f761dc1-d6eb-4ad9-9326-f88a181cdb93', 'c5857832-2e0b-46b0-aa12-7f681d93a3f0', 'How Much or How?- A Beginners guide to training', 'Cardio', 'Cardio', 1785203, 77008,'2023-07-01','2024-12-03',1,7,4, 'You have thought of these questions too many times as a Beginner  Some of you would have got an answer but most of you are still not so sure about it  I promise you that you will have your answer in this article  undefined')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('b6711096-cf8e-48f7-8a7b-97f5900ed754', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'The Ultimate Guide to Vegetarian Iron-Rich Foods', 'Stretching','Stretching', 1873682, 885386,'2023-05-04','2024-12-09',55,5,2, 'The Mediterranean diet, inspired by the traditional eating patterns of countries bordering the Mediterranean Sea, has gained worldwide recognition for its health benefits  This diet emphasizes whole, minimally processed foods and healthy fats, making it not only nutritious but also delicious and easy to follow  In this article, we delve into the Mediterranean diet pyramid, a comprehensive foods list, its benefits, and the considerations and risks associated with this eating pattern 
Introduction to the Mediterranean Diet
The Mediterranean diet reflects the culinary traditions of countries like Greece, Italy, and Spain, which have lower rates of chronic diseases and higher life expectancy compared to other Western countries  This diet is characterized by its emphasis on plant-based foods, healthy fats, and moderate consumption of animal products')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ce3cb7fc-ec00-4271-af7b-8168ec1d03ec', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Why Glycemic Index is Flawed: The Importance of Glycemic Load', 'Cross fit','Cross fit', 1204075, 1648066,'2023-03-11','2024-11-27',11,4,0, 'Paneer, or Indian cottage cheese, is a popular ingredient in many Indian dishes  It&#8217;s not only delicious but also packed with nutrients, making it an excellent choice for those looking to lose weight  Paneer is particularly beneficial for vegetarians as it is a great source of protein, which is essential for muscle repair and growth  In this article, we&#8217;ll explore the nutritional benefits of paneer and share some healthy paneer recipes that are perfect for weight loss 
Nutritional Benefits of Paneer
Paneer is rich in protein, calcium, and healthy fats')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('aecba5b6-fb7c-4513-8671-6a3fb77db224', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'What Causes Diabetes: Exploring the Factors Behind this Chronic Condition', 'Pilates','Pilates', 88836, 1422720,'2023-08-12','2024-12-12',9,5,2, 'Polycystic Ovary Syndrome (PCOS) is a common endocrine disorder that affects millions of women worldwide  One of the most prevalent concerns among those with PCOS is weight gain  This article aims to delve into the relationship between PCOS and weight gain, explore the underlying mechanisms, and provide strategies for managing weight effectively 
Understanding PCOS
PCOS is a hormonal disorder characterized by irregular menstrual cycles, excess androgen levels, and polycystic ovaries  It can lead to various symptoms, including acne, hirsutism (excessive hair growth), and weight gain')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ac385087-ff3b-42a4-af39-12e67bd8ee0c', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'The Ultimate Guide to a Diabetic Diet: Managing Diabetes through Nutrition', 'Yoga', 'Yoga', 466265, 595874,'2023-12-06','2024-12-15',27,1,5, 'Drinking water is essential for overall health, but can the temperature of the water you drink make a difference in your weight loss journey? Many people believe that hot water can aid in weight loss by boosting metabolism, suppressing appetite, and improving hydration  This article delves into the potential benefits of drinking hot water for weight loss, examines scientific evidence, and offers practical tips for incorporating hot water into your diet 
How Hot Water Might Aid Weight Loss
Increased Metabolism
One of the primary theories behind drinking hot water for weight loss is its potential to increase metabolism  When you drink hot water, your body must expend energy to cool it down to body temperature  This process, known as thermogenesis, can result in a temporary boost in metabolism')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('37cceca6-e284-45d7-98ac-05a7c3f0e9ed', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Weight Loss Diet Simplified: Make Your Own Diet Plan Or Chart', 'Cardio', 'Cardio', 1442963, 774467,'2024-07-13','2024-11-24',26,5,3, 'Managing blood sugar levels is critical for overall health, particularly for individuals with diabetes or those at risk of developing it  High blood sugar, or hyperglycemia, can lead to serious health complications over time, including heart disease, kidney damage, and nerve issues  While medications are often necessary, many natural strategies can effectively control blood sugar levels  This comprehensive guide explores various natural methods to help you maintain healthy blood sugar levels 
Understanding Blood Sugar
What is Blood Sugar?
Blood sugar, or blood glucose, refers to the amount of glucose present in your bloodstream')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1e688ed6-10a7-437b-8f7e-8d6ac6b318cc', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Calcium: A Beginners Guide to This Essential Mineral', 'Cross fit','Cross fit', 573263, 1637981,'2024-09-28','2024-12-11',48,5,3, 'Managing triglycerides is crucial for maintaining heart health and overall well-being  Elevated triglyceride levels can increase the risk of heart disease and other health complications  This comprehensive guide explores effective strategies to reduce triglycerides, combining dietary changes, lifestyle modifications, medical interventions, and natural remedies 
Understanding Triglycerides
What are Triglycerides?
Triglycerides are a type of fat (lipid) found in your blood  They are essential for energy storage and transportation of fat in the bloodstream')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ac71448f-15b5-4913-84be-8310cc1f6aef', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'How to Read Nutrition Labels on Food: A Beginners Guide', 'Strength training','Strength training', 1705177, 1880682,'2023-11-11','2024-12-18',7,2,2, 'Detox diets have gained immense popularity, with promises to cleanse your body, boost energy, and aid in weight loss  But how effective are these claims? Are detox diets truly beneficial, or are they just another health fad? This article explores the science behind detox diets, their benefits, myths, and potential risks 
What is a Detox Diet?
Detox diets are short-term dietary interventions designed to eliminate toxins from your body  These diets typically involve a period of fasting, followed by a strict diet of fruits, vegetables, fruit juices, and detox water  Sometimes, detox diets also include herbs, zero calorie teas, supplements, and colon cleanses or enemas')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('34ab879f-0737-4f44-ba6a-73da19a361e8', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Understanding Lactose Intolerance: Symptoms, Causes, and Management', 'Cardio', 'Cardio', 19565, 131404,'2022-12-24','2024-12-17',36,3,1, 'Detox drinks have become incredibly popular, promising to cleanse your body, boost your energy, and aid in weight loss  But what is the truth behind these claims? Are detox drinks truly effective, or are they just another health fad? This article explores the science behind detox drinks, their benefits, myths, and potential risks 
What Are Detox Drinks?
Detox drinks are beverages made from a combination of fruits, vegetables, herbs, and spices  They are marketed as a way to remove toxins from the body, promote weight loss, and improve overall health  Common ingredients in detox drinks include lemon, ginger, cucumber, mint, and various leafy greens')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('8bc57e07-7df4-4eaa-80e6-331a808c07f6', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Can Type 2 Diabetes Be Cured Permanently?', 'Cardio', 'Cardio', 747506, 1664224,'2023-06-10','2024-11-30',7,6,1, 'People love sweet things, and jaggery and refined sugar often come up in discussions about healthier alternatives  Both are popular in Indian cuisine, but they differ significantly in terms of production, nutritional content, and health impacts  There’s also a raging online debate about how jaggery is better than sugar  This article delves into whether jaggery is better than sugar by comparing their benefits, drawbacks, and overall impact on health 
What is Jaggery?
Jaggery is a traditional unrefined sweetener made primarily from sugarcane juice or palm sap')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('32812866-31d3-4c7b-a48e-2817ec86a672', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Flexible Dieting: A Beginners Guide to Eating for Health and Fitness', 'Strength training','Strength training', 1487747, 729433,'2023-10-31','2024-11-22',30,5,0, 'If you’re a foodie who’s decided to get fit, we sympathise with you! Most so-called ”healthy” recipes will rob you of flavour, taste and the will to live  And if you’re a vegetarian, then all the best getting your protein  
Here’s a hot-take: healthy eating does NOT need to be boring! Vegetarian food CAN be protein-rich  And we’re here to prove it  
Each of these recipes is low-calorie, vegetarian or eggetarian, protein-rich and—the best part—super easy to make! We’ve also listed the full calories and macros for each recipe so that you know exactly how to make it fit into your diet')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ee4089b9-4963-47bb-b78a-33575db40b06', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'How to Lose Weight with Hypothyroidism?', 'Yoga', 'Yoga', 1220228, 648780,'2023-05-27','2024-12-13',20,8,2, 'As long as there are people who want to lose weight and get fit, there will exist myths, misinformation and so-called “magical” supplements  One of the latest fitness trends is Detox Water  This colourful and flavorful beverage, often infused with fruits, vegetables, and herbs, is touted for its supposed detoxifying and weight loss benefits  But how much of this is backed by science? Let’s delve into the myths and realities surrounding detox water for weight loss 
What is Detox Water? 
Detox water is simply regular water that’s been infused with natural ingredients like fruits, vegetables, and herbs')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('970526ec-f465-42e1-9603-2f454e4e7f5c', 'cde52692-05d8-43bc-b12b-9b82ee32afb3', 'Do certain carb sources make you feel bloated?', 'Cross fit','Cross fit', 861554, 111822,'2024-02-09','2024-12-07',25,4,1, 'Fibre is a vital component found in plant-based foods that plays a crucial role in digestive health, weight management, and reducing the risk of chronic diseases  Incorporating high fiber foods into your diet is not only beneficial but also culturally enriching  This guide explores different types of fiber, daily fiber recommendations, a variety of fiber-rich foods commonly enjoyed in India, and includes easy-to-prepare high fiber recipes, both vegetarian and non-vegetarian 
Understanding Dietary Fibre: Your Gut&#8217;s Best Friend
Dietary fiber refers to the indigestible portion of plant foods that aids in digestion, absorbs water, and helps move waste through the digestive tract  Unlike other food components such as fats, proteins, or carbohydrates, fiber passes through the body relatively intact')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('56317663-0512-47d5-abfc-644fec756a12', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Screening for Vit D Deficiency', 'Stretching','Stretching', 825621, 1066061,'2023-07-14','2024-12-06',30,6,0, 'What’s life without a little sweetness, right? Sugar is everywhere—and despite the rising popularity of artificial sweeteners, it’s not going away anytime soon  In 2023-24, Indians consumed a whopping 30 million metric tonnes of sugar  Depending on whom you ask, sugar is completely harmless or a toxin that’s as addictive as cocaine  The truth, as always, lies in the details  
This article explores the difference between refined sugar and natural sugars, the sources of each type, their health impacts, and how to make better choices for your health and longevity')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('30a2b957-db87-4dd8-9250-4417a9017d04', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Why is excess salt not good for health?', 'Cross fit','Cross fit', 839900, 940731,'2022-12-28','2024-12-15',1,5,3, 'Billions of people around the world eat rice every single day  It’s been a staple part of the Indian diet for centuries if not millennia  And yet, rice keeps getting blamed for everything from causing diabetes to making us gain weight  But is this truly the case?
This article will delve into the nutritional aspects of rice, compare different types of rice, and debunk common myths to help you make informed dietary choices 
Rice is Nice!
Rice, chaawal, bhaat &#8211; known by different names, loved all the same')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('bda2b774-d1a1-4690-83ef-f5cc1bfe1f2f', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Eating 1000 kcal vs cutting 500 kcal for weight loss', 'Strength training','Strength training', 1999801, 1768610,'2023-09-07','2024-12-03',28,2,2, 'Electrolyte drinks are all the rage these days  These beverages are often associated with athletes and high-intensity activities, but their benefits extend far beyond the sports field  People have even started using these drinks as a way to beat the summer heat  But what exactly are electrolyte drinks? Let’s understand their benefits, potential downsides, and how to know which is the right one for you 
What Are Electrolyte Drinks?
Electrolyte drinks are beverages designed to replenish the body&#8217;s electrolyte levels, which are crucial minerals including sodium, potassium, calcium, magnesium, chloride, and bicarbonate')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1487bdcc-0b76-4103-9d71-091b36919199', 'd04408a7-b69f-4422-ace1-50a9a532f324', '10 diabetes myths you shouldnt believe', 'Stretching','Stretching', 935852, 1398059,'2023-07-09','2024-12-05',20,5,3, 'Are you tempted by the promise of quick weight loss offered by fad diets? You&#8217;re not alone  The allure of shedding pounds rapidly can be compelling, but these diets often come with hidden dangers and misconceptions  This article will explore various types of fad diets, their harmful effects, and how to make healthier, more informed choices 
Understanding Fad Diets
Fad diets are popular weight loss plans that promise dramatic results in a short amount of time  They are often characterised by restrictive eating patterns, the elimination of entire food groups, and the promise of quick fixes')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('55dadfd9-a424-4b5e-8e01-7fd51f6c0872', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Journey from prediabetes to diabetes', 'Strength training','Strength training', 896811, 277189,'2023-08-10','2024-11-30',21,1,4, 'If you want to lose weight and stay fit, understanding how to measure calories in your food is an invaluable skill  Measuring or quantifying food enables you to track your calories and macros, enhances your nutritional awareness, and helps you make more informed dietary choices  
&nbsp;
In this guide, we&#8217;ll explore practical methods for measuring calories in your food right in the comfort of your home  These methods, when utilised properly and followed consistently, will help you stay on track and achieve your fitness goals, whatever they may be 
1')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('1a7943d1-8e80-4d17-bd1b-30efe5109619', 'd04408a7-b69f-4422-ace1-50a9a532f324', '10 Myths about hypertension one must know', 'Yoga', 'Yoga', 1191623, 828926,'2023-06-25','2024-12-16',60,1,2, 'Are you a vegetarian in India looking to build muscle but concerned about your protein intake?  It’s a common misconception that vegetarians will always struggle to meet their protein needs, and find it challenging to build muscle  This article aims to dispel that myth by highlighting accessible, protein-rich vegetarian foods and smart dietary combinations to help you achieve your fitness goals 
Understanding Protein in a Vegetarian Diet
What is Protein?
Proteins are complex molecules made up of amino acids, which are the building blocks of life  They play a crucial role in virtually every biological process within the body  There are 20 different amino acids, nine of which are considered essential because the body cannot produce them; they must be obtained through the diet')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('de3d8846-44bc-4589-94ef-145fd6086b25', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Does high salt or sodium intake contribute to hypertension?', 'Cardio', 'Cardio', 1933382, 1887618,'2023-04-30','2024-11-28',38,8,0, 'One of the latest foods to be called a “superfood” is the humble Makhana or Fox Nuts  Truth be told, it’s not a new food at all &#8211; we’ve been eating in India as a snack for decades  But it is making a global comeback, thanks to the craze for “diet-friendly” foods  At the same time, makhana isn’t without its share of misconceptions  
&nbsp;
Today, let&#8217;s dive deep into the world of makhana, debunk myths, and see if this humble snack can be a game-changer in your health journey')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ca5163c4-781a-485e-a614-4bf6ecf2c99e', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'What makes LDL bad, and HDL good cholesterol?', 'Cardio', 'Cardio', 237615, 1128871,'2024-07-05','2024-12-05',35,9,1, 'Cholesterol levels have a direct impact on cardiovascular health  Studies indicate that in India, about 25-30% of the urban populace and 15-20% of the rural populace have high cholesterol 
With a global shift towards understanding and preventing heart disease, it’s vital to understand how your diet and lifestyle choices can influence cholesterol levels 
This article delves into the essentials of cholesterol management, providing a roadmap to navigate the complexities of dietary and lifestyle modifications aimed at fostering heart health 
What is Cholesterol?
Cholesterol is often vilified, but is actually a vital substance needed by the body')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('ceae4aa2-22ab-4542-9190-39f5fbc39674', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Rheumatoid Arthritis - A challenge for weight loss', 'Stretching','Stretching', 523248, 577645,'2024-03-25','2024-12-21',38,7,4, 'In the realm of health and fitness, we often encounter a plethora of knowledge, particularly in the field of bodybuilding  When it comes to workout splits, recovery days, and training intensity, different approaches emerge based on whether one follows a natural or enhanced bodybuilding path  In this article, we aim to shed light on the contrasting concepts between natural and enhanced bodybuilding, highlighting the importance of individualized approaches and understanding the impact of external factors on training and recovery 
Understanding Natural Bodybuilding:
Natural bodybuilding refers to the pursuit of muscle growth and physique enhancement without the use of performance-enhancing substances  For natural bodybuilders, the emphasis lies in maximizing their physique&#8217;s potential while adhering to their body&#8217;s inherent capabilities')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e157a879-8da8-49ef-933d-1b075909f242', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Dealing with type 2 diabetes', 'Cardio', 'Cardio', 1369752, 1043837,'2024-05-05','2024-12-14',19,4,4, 'Are you among those who believe carbohydrates are the enemy, guaranteed to make you fat and unhealthy? You&#8217;re not alone  Many people harbour this misconception, fearing that carbs are detrimental to their health and fitness goals  This article will demystify carbohydrates, explaining their importance, different types, and how to incorporate them healthily into your diet 
What Are Carbohydrates?
Carbohydrates are one of the three macronutrients essential to our diet, alongside proteins and fats  They are the body&#8217;s primary source of energy, fueling everything from our daily activities to the functioning of our organs and brain')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e30ac57d-51a8-47bf-a9c0-4ec7cfae20da', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'Common health implications in shift workers', 'Cross fit','Cross fit', 1404036, 1666624,'2023-12-13','2024-12-05',10,7,4, 'Magnesium is one of the six macro minerals that plays an important role in various physiological functions in the body  In this article, let&#8217;s understand how magnesium works for health and its role in various metabolic functions 
Role of Magnesium

Magnesium is most abundantly found in the skeleton (60%), muscles and soft tissues (40%), and less than 1% in blood  As a cation, it is involved in the regulation of most physiological functions in the cells  It is involved in more than 300 enzymatic reactions in the metabolic processes which help in the production of energy, protein synthesis, hormone secretion, muscle contraction, maintaining membrane integrity, and more')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('05898bfc-183d-48fd-a903-e31b84357134', 'd04408a7-b69f-4422-ace1-50a9a532f324', 'How to manage psoriasis with the right diet?', 'Pilates','Pilates', 85851, 151839,'2023-03-04','2024-12-07',45,5,3, 'Sweet potatoes are a versatile and delicious vegetable that often finds its way onto our plates  But what about their suitability for people with diabetes? Are they a good choice, or should they be avoided? In this article, we&#8217;ll explore the potential benefits of sweet potatoes for diabetes and how to incorporate them into your diabetic diet 
Why Sweet Potatoes Are Apt for Diabetes?
Sweet potatoes have earned a reputation as one of the most nutritious subtropical and tropical vegetables  They have been used in traditional medicine for managing type 2 diabetes, and recent research has shed light on their potential benefits 


High Fibre Content in Sweet Potatoes 
Half a cup of boiled sweet potatoes provides around 4 grams of fibre, consisting of both soluble and insoluble fibre')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d8b66fe0-e20f-4a1f-a76a-5f9e59239bc3', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Why does severe dehydration lead to fatigue?', 'Pilates','Pilates', 728712, 1131036,'2023-12-01','2024-12-01',36,1,2, 'Carbohydrates are often found at the centre of heated debates around healthy living and weight loss  Many individuals believe that consuming carbohydrates can lead to weight gain  But is there any scientific truth to this notion? Let&#8217;s delve into the intricacies of how carbohydrates are processed in the body and whether they truly contribute to weight gain 
The Big Carbohydrate and Weight Gain Myth
Myth: Many people think that carbohydrates make them gain weight 
Reality: To understand the relationship between carbohydrates and weight gain, we need to explore the complex processes that occur when you consume these macronutrients')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d89568f8-ed52-4557-8ee1-766de9df0a72', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'How to manage excercise induced asthma?', 'Cardio', 'Cardio', 1953666, 1309775,'2023-04-05','2024-11-21',31,8,1, 'In the realm of weight loss strategies, water fasting has emerged as a captivating yet controversial topic  This unique approach, rooted in abstaining from all caloric consumption while exclusively relying on water intake for a designated period, promises rapid weight loss and potential health benefits  Nonetheless, as with any dietary method, the concept of water fasting requires a comprehensive understanding encompassing its pros and cons for a well-rounded evaluation 
Understanding Water Fasting:
Water fasting is often heralded as a means of detoxifying the body, involving the complete cessation of food intake while allowing only water to be consumed  Advocates propose that this technique prompts significant weight loss through a considerable caloric deficit')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('f86b65b4-f71f-4b67-a374-5a30f6dfbbc8', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'How excercise can lead to asthma attack?', 'Cross fit','Cross fit', 434037, 1098207,'2024-01-05','2024-11-30',6,4,3, 'The Carnivore Diet, also known as the all-meat diet, is a dietary approach that has gained significant attention in recent years  Dr  Jordan Peterson has claimed that a variation of this diet has completely changed his health; yet, critics express concerns about its nutritional adequacy and long-term implications 
In this comprehensive article, we will explore the Carnivore Diet, its origins, potential benefits and risks, and analyse scientific evidence to determine if it is a viable and sustainable dietary option 
Understanding the Carnivore Diet
The Carnivore Diet is a high-fat, low-carbohydrate diet that exclusively consists of animal-based foods')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('01e0a949-b652-4288-835c-d095b76285fa', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Morning sickness - ways to tackle it', 'Stretching','Stretching', 1673423, 182526,'2023-12-19','2024-11-24',14,3,3, 'Dark chocolate and wine have long been touted for their potential health benefits, thanks to their polyphenol and antioxidant content, particularly resveratrol  These claims have captured the attention of health enthusiasts and wine connoisseurs alike  However, when we delve deeper into the research, the evidence supporting these health claims becomes less substantial  In this blog article, we will explore the truth behind the alleged benefits of dark chocolate and wine, their polyphenol and antioxidant properties, and the actual quantities required to reap any potential positive effects on the body 
The Promise of Polyphenols and Antioxidants
Understanding Polyphenols and Antioxidants
Polyphenols are naturally occurring compounds found in various plant-based foods, including dark chocolate and wine')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e43d0c5c-a21e-4fdb-b8af-44ca6206e87b', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Fibromayalgia & lifestyle management', 'Pilates','Pilates', 1696255, 514161,'2023-10-24','2024-11-29',56,5,2, 'In the quest for shedding those extra pounds and embracing a healthier lifestyle, salads stand as undisputed champions  These vibrant, nutrient-packed bowls of greens have become synonymous with weight loss journeys across the globe  Whether you&#8217;re a fan of garden-fresh vegetables, succulent fruits, or protein-rich ingredients, salads offer an endless canvas of possibilities for your culinary and fitness endeavours  In this comprehensive guide, we will explore the diverse world of salads for weight loss, from vegetable and fruit combinations to flavorful Indian recipes and lean protein-packed creations  Let&#8217;s dive into the world of salads that tantalize taste buds while supporting your weight loss diet')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('974fd87c-b0f5-4968-900c-621045b0d1c3', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Diet considerations in kidney stones', 'Stretching','Stretching', 250753, 1476293,'2024-06-01','2024-11-26',53,9,2, 'The Importance of a Healthy Breakfast:
If you’re looking for low calorie Indian breakfast ideas, you’ve come to the right place!
While it’s not mandatory to eat breakfast, a wholesome breakfast can help you kickstart your day  Indian breakfast recipes can be both satisfying and beneficial for your weight loss journey,  as they have a blend of flavours, spices, and wholesome ingredients 
A traditional Indian breakfast can sometimes be high in calories but it doesn’t have to be that way  If you choose the right ingredients and keep an eye on calories and macros, you can ensure that your breakfast aids your weight loss journey 
In this article, we will explore 15 delicious and healthy Indian breakfast recipes that can help you shed those extra pounds')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('c8ee397f-4eb0-42e3-82ed-24e95a61b355', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'What are autoimmune disorders?', 'Yoga', 'Yoga', 1172296, 1297039,'2023-10-19','2024-12-20',18,2,3, 'The pursuit of weight loss often prompts the question: which factor contributes more significantly, diet or exercise? This query reflects the intricate interplay between two crucial elements of a healthy lifestyle  While both diet and exercise play pivotal roles in achieving weight loss goals, understanding their respective influences can guide individuals towards a balanced approach that optimizes results  In this article, we explore the percentages and nuances of weight loss attributed to diet and exercise 
Diet vs  Exercise: The Numbers Game:

 The Importance of Diet:

Numerous studies suggest that weight loss is predominantly influenced by dietary choices')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('abdb23a3-e949-471e-8830-cac30d4af60a', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'How to prevent childhood obesity?', 'Cardio', 'Cardio', 1867292, 807590,'2024-09-19','2024-12-20',27,5,2, 'If your goal is to lose weight, then including low-calorie foods in your diet can be immensely helpful  These foods help you reduce your overall calories and provide essential nutrients for a healthy and balanced lifestyle 
In this article, let’s explore a variety of low-calorie foods that are perfect for weight loss, along with easy-to-make Indian recipe ideas that will delight your taste buds 
Understanding the Basics of Low-Calorie Foods
Whether you want to lose weight or belly fat, creating and maintaining a calorie deficit is crucial  Low-calorie foods contain relatively fewer calories per serving, so you can have larger portions without exceeding your caloric limits')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('e82bd4ac-a1cd-49dc-88be-c574addf0534', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Why is it important to prevent childhood obesity?', 'Cross fit','Cross fit', 1394657, 949292,'2023-11-14','2024-12-04',17,5,4, 'If you want to lose weight and keep it off, you need to create a calorie deficit  A well-balanced diet that contains all three macronutrients &#8211; protein, carbohydrates and fats &#8211; is sustainable, enjoyable and plays a crucial role in helping you keep the weight off  
Protein, in particular, should not be ignored  High-protein foods not only provide essential nutrients but also promote satiety, enhance metabolism, and preserve muscle mass  Whether you are a vegetarian or non vegetarian you need to know how much protein do you need per day')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('d0b28e85-fab3-4f0e-b952-c0454443c496', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Can quality focused eating help weight loss?', 'Cardio', 'Cardio', 1732668, 1011256,'2023-11-21','2024-12-10',36,2,5, 'Different people have different opinions about which cooking oil is the best  But who’s right and who’s wrong?
In this article, we will delve into the criteria for measuring the quality of cooking oils, including omega-3 and omega-6 fatty acids, polyunsaturated fatty acids (PUFA), monounsaturated fatty acids (MUFA), and the smoking point, to help you make informed decisions about the best oils to use in your culinary endeavours 
Before getting into the article watch this short video from our Founder, Jitendra Chouksey shares his recommendations on cooking oils, offering valuable insights into their properties, health considerations, and how to make informed choices when it comes to selecting the best oil for your cooking needs  Check out his video on the best cooking oils for Indian cooking')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('deb16b8d-966c-4f2b-9305-e454cb449efd', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Subclinical hypothyroidism - a consequence of obesity', 'Cross fit','Cross fit', 1233882, 1757505,'2024-12-12','2024-11-29',39,4,4, 'As the popularity of alkaline water continues to rise, so do the claims surrounding its health benefits and its supposed ability to improve overall well-being  However, it&#8217;s crucial to critically examine these assertions and distinguish between fact and fiction 
In this article, we will debunk common myths surrounding alkaline water and provide a scientific perspective on its actual impact  So, let&#8217;s dive in and uncover the truth about alkaline water and its benefits 
What is Alkaline Water: An Overview


Alkaline water is water with a higher pH level than regular tap water, typically ranging from 8 to 9 on the pH scale')
GO
Insert into Courses(Course_ID, Trainer_ID, Title, Category, Type, Price, Additional_Price, Start_Date, End_Date, Duration, Max_Participants, Rating, Description) Values ('6033dcfb-4cf8-4692-a32b-112611425fdf', 'e8c6f2f5-9a8f-492d-bc42-57bc271bc22a', 'Simple hacks to combat fungal infections', 'Yoga', 'Yoga', 831569, 1097265,'2024-04-06','2024-12-10',16,9,1, 'In today&#8217;s fast-paced world, maintaining a healthy weight can be challenging  However, with the right approach and smart choices, you can achieve your weight loss goals  
Snacking can play a crucial role in a balanced diet, and incorporating nutritious options can keep you satiated and help you shed those extra pounds  
In this article, we will explore a variety of delicious weight loss snacks, with a focus on Indian cuisine  These snacks are not only easy to prepare but also packed with essential nutrients to support your weight loss diet')

-- Course_Options
insert into Course_Options (Option_ID, Option_Name, Description, Price) Values('23020679-6d51-4556-92da-5c2163a0a5ee','Targeted Program','Focus on a specific area such as the abdomen, buttocks, arms, or back. Suitable for people who want to improve a specific body part.',45000.00)
go

insert into Course_Options (Option_ID, Option_Name, Description, Price) Values('231f5348-d91a-4808-b57d-9d0b959f6c36','Muscle Building Program','Focus on weight training, protein-rich nutrition. Detailed instructions on how to gain muscle safely.',200000.00)
go

insert into Course_Options (Option_ID, Option_Name, Description, Price) Values('26a10a3b-c14f-44c1-b22f-5d6177746c57','Weight Loss Program','Combines cardio, strength training and nutrition. Effectively supports total body fat loss.',1000000.00)
go

insert into Course_Options (Option_ID, Option_Name, Description, Price) Values('2f61dccf-e935-4835-93bf-246a8bdd470c','Yoga/Pilates','Increases flexibility, reduces stress. Suitable for people who need to improve flexibility and balance.',600000.00)
go


-- Course_Registration_Options


-- Course_Rating, Desciptions


-- Course_Registration


-- image course

Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1a7943d1-8e80-4d17-bd1b-30efe5109619','008ed810-fe46-4cf3-b7a1-e9ba60924fcd.jpg','http://localhost:8000/imagecourses/008ed810-fe46-4cf3-b7a1-e9ba60924fcd.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('05898bfc-183d-48fd-a903-e31b84357134','00e8120c-bf8b-4d82-8acc-1ef632d50b49.jpg','http://localhost:8000/imagecourses/00e8120c-bf8b-4d82-8acc-1ef632d50b49.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('06dcba2a-5171-4e22-8462-82d7bf8504f2','0156eb5b-8a47-41ca-a0b5-4a1c781328ee.jpg','http://localhost:8000/imagecourses/0156eb5b-8a47-41ca-a0b5-4a1c781328ee.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0b89d622-63eb-448e-898e-1b531f4cec3b','016a4ca9-5631-46f8-90c9-f5799f64d5e1.jpg','http://localhost:8000/imagecourses/016a4ca9-5631-46f8-90c9-f5799f64d5e1.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('24541e22-7217-49de-8347-19446c36fa49','019d84f4-f438-45df-b274-cee538d8d701.jpg','http://localhost:8000/imagecourses/019d84f4-f438-45df-b274-cee538d8d701.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('24541e22-7217-49de-8347-19446c36fa49','01ce849c-d6c0-4e7d-81ae-080167d39b58.jpg','http://localhost:8000/imagecourses/01ce849c-d6c0-4e7d-81ae-080167d39b58.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0a800eea-7b71-49ce-8c11-94b91346d8ec','029a6fb1-cd0f-460d-a200-24199d1bc3fd.jpg','http://localhost:8000/imagecourses/029a6fb1-cd0f-460d-a200-24199d1bc3fd.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('02b6d198-aa5d-4439-85ef-62ecbf67edf6','046352f5-e141-4d3a-943f-976f3f9fb688.jpg','http://localhost:8000/imagecourses/046352f5-e141-4d3a-943f-976f3f9fb688.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1da278e1-85ec-4a5a-b3b2-5a476c62e8ab','0659ff4b-b4a7-4149-9a15-d3b9a3b52489.jpg','http://localhost:8000/imagecourses/0659ff4b-b4a7-4149-9a15-d3b9a3b52489.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0b89d622-63eb-448e-898e-1b531f4cec3b','065a2b9d-207f-42ac-a9ee-77c505717819.jpg','http://localhost:8000/imagecourses/065a2b9d-207f-42ac-a9ee-77c505717819.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('05898bfc-183d-48fd-a903-e31b84357134','067a47e3-ba2c-4a50-9981-896b8dfc8223.jpg','http://localhost:8000/imagecourses/067a47e3-ba2c-4a50-9981-896b8dfc8223.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('06dcba2a-5171-4e22-8462-82d7bf8504f2','083250d8-399b-4f6e-bbab-a7900682f12a.jpg','http://localhost:8000/imagecourses/083250d8-399b-4f6e-bbab-a7900682f12a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('16d4e9e9-a8e1-4c47-99ef-c77936a7c284','09fa5e2b-ecef-4302-b8cf-aea7fa027586.jpg','http://localhost:8000/imagecourses/09fa5e2b-ecef-4302-b8cf-aea7fa027586.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('02b6d198-aa5d-4439-85ef-62ecbf67edf6','0acae4ba-ad4c-4777-98d4-b4efab13386a.jpg','http://localhost:8000/imagecourses/0acae4ba-ad4c-4777-98d4-b4efab13386a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('01e0a949-b652-4288-835c-d095b76285fa','0b7e6d09-177f-421e-b984-4dc1f76c754b.jpg','http://localhost:8000/imagecourses/0b7e6d09-177f-421e-b984-4dc1f76c754b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0f52f5ef-689e-4e97-9c52-68b5e852226d','0cd5b57d-221c-4a8a-8c02-2c0c78377b6a.jpg','http://localhost:8000/imagecourses/0cd5b57d-221c-4a8a-8c02-2c0c78377b6a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0f6d52bf-2d62-4b52-b1a5-9b35d67faf01','0ee605ad-3bbf-49c5-aaca-028e39d37729.jpg','http://localhost:8000/imagecourses/0ee605ad-3bbf-49c5-aaca-028e39d37729.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1ba1559c-9b4c-4945-90d7-dda07de89558','106c22a9-a812-41a9-9ba9-e827719437a0.jpg','http://localhost:8000/imagecourses/106c22a9-a812-41a9-9ba9-e827719437a0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1a7943d1-8e80-4d17-bd1b-30efe5109619','107c5296-1ce5-48f3-8589-6f3aff214fd2.jpg','http://localhost:8000/imagecourses/107c5296-1ce5-48f3-8589-6f3aff214fd2.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('10ef2fb2-586d-44a1-b1cd-5d5a47eeeec9','11094600-edad-4d30-a6b7-6ddb80f4d840.jpg','http://localhost:8000/imagecourses/11094600-edad-4d30-a6b7-6ddb80f4d840.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('18f1756e-ca15-48ed-a8c1-d000b0308ec0','1207d23d-f69a-44ae-a945-657bd47b0752.jpg','http://localhost:8000/imagecourses/1207d23d-f69a-44ae-a945-657bd47b0752.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('10ef2fb2-586d-44a1-b1cd-5d5a47eeeec9','1280a0e0-0df8-4ed2-a44e-0e859517e655.jpg','http://localhost:8000/imagecourses/1280a0e0-0df8-4ed2-a44e-0e859517e655.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('16d4e9e9-a8e1-4c47-99ef-c77936a7c284','1571fbf7-a594-435c-8821-b0b52eff1f4e.jpg','http://localhost:8000/imagecourses/1571fbf7-a594-435c-8821-b0b52eff1f4e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('0f52f5ef-689e-4e97-9c52-68b5e852226d','1787fc07-f280-4665-8e42-517d448d3837.jpg','http://localhost:8000/imagecourses/1787fc07-f280-4665-8e42-517d448d3837.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('01e0a949-b652-4288-835c-d095b76285fa','17b5ba2b-075c-4570-8ac8-ec594eba4aa0.jpg','http://localhost:8000/imagecourses/17b5ba2b-075c-4570-8ac8-ec594eba4aa0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1487bdcc-0b76-4103-9d71-091b36919199','17ca2a4c-29d6-4714-9c2c-ba1efc95be3f.jpg','http://localhost:8000/imagecourses/17ca2a4c-29d6-4714-9c2c-ba1efc95be3f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1bb56cff-df98-471f-bc6c-4bf09b2dbde7','1895b674-431f-4210-88b0-90dad8d795be.jpg','http://localhost:8000/imagecourses/1895b674-431f-4210-88b0-90dad8d795be.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1e688ed6-10a7-437b-8f7e-8d6ac6b318cc','189cb387-1ef6-4292-b96f-3c86ee3d2ef3.jpg','http://localhost:8000/imagecourses/189cb387-1ef6-4292-b96f-3c86ee3d2ef3.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('32812866-31d3-4c7b-a48e-2817ec86a672','1a2b4129-32ca-4fae-a121-38f8acc11497.jpg','http://localhost:8000/imagecourses/1a2b4129-32ca-4fae-a121-38f8acc11497.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1bb56cff-df98-471f-bc6c-4bf09b2dbde7','1d2faecf-6f1e-4a42-ac2e-78f8c627eb4a.jpg','http://localhost:8000/imagecourses/1d2faecf-6f1e-4a42-ac2e-78f8c627eb4a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1da278e1-85ec-4a5a-b3b2-5a476c62e8ab','1e15e722-f075-4fa8-88f5-c27956323fcd.jpg','http://localhost:8000/imagecourses/1e15e722-f075-4fa8-88f5-c27956323fcd.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2a36b546-9543-498f-b5ec-96e5922a96f9','1fbe06c1-3eaa-4390-bfdd-c40228927501.jpg','http://localhost:8000/imagecourses/1fbe06c1-3eaa-4390-bfdd-c40228927501.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2a36b546-9543-498f-b5ec-96e5922a96f9','20d0b817-7f25-4d37-b84e-d75472ac70b9.jpg','http://localhost:8000/imagecourses/20d0b817-7f25-4d37-b84e-d75472ac70b9.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('33c1d90c-73ed-4b13-bf1b-b4a3f2aab1f4','2103468a-ca28-4372-bbe3-6c1ba825cdcc.jpg','http://localhost:8000/imagecourses/2103468a-ca28-4372-bbe3-6c1ba825cdcc.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('5b4c801f-8672-4925-ad85-f0f52e588cd1','217a885e-386e-429a-8be4-da39efee41da.jpg','http://localhost:8000/imagecourses/217a885e-386e-429a-8be4-da39efee41da.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('3da4a5f0-7ef3-4726-a4d2-485b15513bb4','21b09274-c229-4cee-9c6a-e5a29cec937a.jpg','http://localhost:8000/imagecourses/21b09274-c229-4cee-9c6a-e5a29cec937a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('20237c27-9863-40ba-ac9c-a64e7adc1a64','2238460f-7784-4405-85b1-c56d3e5fd115.jpg','http://localhost:8000/imagecourses/2238460f-7784-4405-85b1-c56d3e5fd115.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4261e197-bd04-4163-ae9f-365d47651209','237d90e8-aab8-4427-9045-1a25930d02a9.jpg','http://localhost:8000/imagecourses/237d90e8-aab8-4427-9045-1a25930d02a9.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('3da4a5f0-7ef3-4726-a4d2-485b15513bb4','23802f89-a855-4545-99d3-b4bf3ce9b6fc.jpg','http://localhost:8000/imagecourses/23802f89-a855-4545-99d3-b4bf3ce9b6fc.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2bb5f8d7-59ef-4d1f-aa36-bb3089c83bd8','295e5c33-9a4c-44ba-85d7-a5c8e88ea0a5.jpg','http://localhost:8000/imagecourses/295e5c33-9a4c-44ba-85d7-a5c8e88ea0a5.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('11d7a6bc-03fb-45ff-ae1e-66fd52cd95cf','2be4a081-822d-459f-b53d-2833f0af363e.jpg','http://localhost:8000/imagecourses/2be4a081-822d-459f-b53d-2833f0af363e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('5b4c801f-8672-4925-ad85-f0f52e588cd1','2e47bd2b-0d04-4aa4-b970-6d0d3060d36f.jpg','http://localhost:8000/imagecourses/2e47bd2b-0d04-4aa4-b970-6d0d3060d36f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('33c1d90c-73ed-4b13-bf1b-b4a3f2aab1f4','2e803e13-a78a-490d-9866-0fe6a654f33d.jpg','http://localhost:8000/imagecourses/2e803e13-a78a-490d-9866-0fe6a654f33d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('42971518-9fd1-4712-b496-71f1ca1a2aaf','2ea938e0-67c6-4939-bf07-bfb24c7bd194.jpg','http://localhost:8000/imagecourses/2ea938e0-67c6-4939-bf07-bfb24c7bd194.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1487bdcc-0b76-4103-9d71-091b36919199','2f31c4a1-7dab-4ca9-84c8-78d8138ba10c.jpg','http://localhost:8000/imagecourses/2f31c4a1-7dab-4ca9-84c8-78d8138ba10c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('32812866-31d3-4c7b-a48e-2817ec86a672','32a34f57-e24c-4b5e-991e-a228c2cd2bd1.jpg','http://localhost:8000/imagecourses/32a34f57-e24c-4b5e-991e-a228c2cd2bd1.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('20237c27-9863-40ba-ac9c-a64e7adc1a64','3354a8ae-1236-451e-a389-c2dd458c3600.jpg','http://localhost:8000/imagecourses/3354a8ae-1236-451e-a389-c2dd458c3600.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4fe4f88b-1a1c-4655-9196-8d5b0ddba084','3572be80-fae5-42fa-b5c8-2a779c8d38d4.jpg','http://localhost:8000/imagecourses/3572be80-fae5-42fa-b5c8-2a779c8d38d4.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('11d7a6bc-03fb-45ff-ae1e-66fd52cd95cf','35920b68-0bcb-475b-9509-b8efa01951f3.jpg','http://localhost:8000/imagecourses/35920b68-0bcb-475b-9509-b8efa01951f3.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('33617dcf-77f7-417e-9265-f79558d4d7e8','362e3d4a-7373-484a-96b8-0d684909801d.jpg','http://localhost:8000/imagecourses/362e3d4a-7373-484a-96b8-0d684909801d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4d4767f8-0e17-4592-a1c5-fb0461a28f33','3676a988-efa5-4ee0-aa5c-77eb00f20225.jpg','http://localhost:8000/imagecourses/3676a988-efa5-4ee0-aa5c-77eb00f20225.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('43ef809a-b2dc-47cc-ade9-79f47fb50624','367a5e58-a85f-4d77-9900-d13d63ff6e1d.jpg','http://localhost:8000/imagecourses/367a5e58-a85f-4d77-9900-d13d63ff6e1d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4fe4f88b-1a1c-4655-9196-8d5b0ddba084','37d4b977-35c1-4ba3-a153-597b52077404.jpg','http://localhost:8000/imagecourses/37d4b977-35c1-4ba3-a153-597b52077404.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('33617dcf-77f7-417e-9265-f79558d4d7e8','386e3308-eaf1-4cba-ab49-a5042a94b351.jpg','http://localhost:8000/imagecourses/386e3308-eaf1-4cba-ab49-a5042a94b351.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('34ab879f-0737-4f44-ba6a-73da19a361e8','38851827-5394-4b75-b97e-cef140c53d22.jpg','http://localhost:8000/imagecourses/38851827-5394-4b75-b97e-cef140c53d22.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4d4767f8-0e17-4592-a1c5-fb0461a28f33','3a5182ae-806b-4878-b943-addf744c6209.jpg','http://localhost:8000/imagecourses/3a5182ae-806b-4878-b943-addf744c6209.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('37ba7a59-2266-4dad-a1e7-d52deee311b7','3a7ce9bc-ca7c-4fd7-a963-2d800ca427d6.jpg','http://localhost:8000/imagecourses/3a7ce9bc-ca7c-4fd7-a963-2d800ca427d6.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('34280e59-78a8-4382-994e-844768530174','3afb3ce8-c0d1-48aa-884c-63269cc530a5.jpg','http://localhost:8000/imagecourses/3afb3ce8-c0d1-48aa-884c-63269cc530a5.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('43ef809a-b2dc-47cc-ade9-79f47fb50624','3c343172-77b3-4b57-b4e0-ede1fd5b673a.jpg','http://localhost:8000/imagecourses/3c343172-77b3-4b57-b4e0-ede1fd5b673a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2b1de040-0bf5-4862-a473-7b324268a136','3cee2f3b-4bb0-471a-a444-6f7868f07f08.jpg','http://localhost:8000/imagecourses/3cee2f3b-4bb0-471a-a444-6f7868f07f08.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('30a2b957-db87-4dd8-9250-4417a9017d04','3e2b815c-5891-47f8-b324-09045f8d3a7d.jpg','http://localhost:8000/imagecourses/3e2b815c-5891-47f8-b324-09045f8d3a7d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2d98ac9c-c78e-4c1a-93ff-bdd4438193dd','3e480298-68d2-4aba-a0e8-caeeb3d3821b.jpg','http://localhost:8000/imagecourses/3e480298-68d2-4aba-a0e8-caeeb3d3821b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('363b681a-2016-4da8-9efb-e0b047325dab','3e53ce3c-2bb2-4d88-bc5c-4d40beb07227.jpg','http://localhost:8000/imagecourses/3e53ce3c-2bb2-4d88-bc5c-4d40beb07227.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('37cceca6-e284-45d7-98ac-05a7c3f0e9ed','3efbb896-5a9b-413c-963b-04682772fffd.jpg','http://localhost:8000/imagecourses/3efbb896-5a9b-413c-963b-04682772fffd.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('363b681a-2016-4da8-9efb-e0b047325dab','477856e2-7b69-4ba3-ab1e-183b35b47799.jpg','http://localhost:8000/imagecourses/477856e2-7b69-4ba3-ab1e-183b35b47799.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('2d98ac9c-c78e-4c1a-93ff-bdd4438193dd','4a572dc1-a94c-47cb-9cdc-771757a0cf8c.jpg','http://localhost:8000/imagecourses/4a572dc1-a94c-47cb-9cdc-771757a0cf8c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('37ba7a59-2266-4dad-a1e7-d52deee311b7','4a91d504-4be9-44e4-a2cf-7cbc463400ec.jpg','http://localhost:8000/imagecourses/4a91d504-4be9-44e4-a2cf-7cbc463400ec.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('4946c22a-08e2-4d12-8387-43f7d0de55b5','4e082a36-4827-4e50-b655-47b97f453748.jpg','http://localhost:8000/imagecourses/4e082a36-4827-4e50-b655-47b97f453748.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('26c1d561-ee7f-40b1-b472-b49cda2b2e2b','4f6ff53c-5560-4b2f-81e1-1881c51591d7.jpg','http://localhost:8000/imagecourses/4f6ff53c-5560-4b2f-81e1-1881c51591d7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('21b77d82-b314-4f9a-b688-015e0c01c4e8','4f739d78-2632-4114-8980-fba51b448406.jpg','http://localhost:8000/imagecourses/4f739d78-2632-4114-8980-fba51b448406.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('30a2b957-db87-4dd8-9250-4417a9017d04','50fd7b6d-01dc-4787-81e0-d2511474ddde.jpg','http://localhost:8000/imagecourses/50fd7b6d-01dc-4787-81e0-d2511474ddde.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('9cc15318-d5b6-4ef9-b607-1fc34d0cef2c','5186360f-fe17-465f-815e-967eab34ea34.jpg','http://localhost:8000/imagecourses/5186360f-fe17-465f-815e-967eab34ea34.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('3aeb3b7e-1211-4cd0-a566-6192528a1315','51ffef21-88a5-4dd6-9668-7f8e4e1cac6b.jpg','http://localhost:8000/imagecourses/51ffef21-88a5-4dd6-9668-7f8e4e1cac6b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('9cc15318-d5b6-4ef9-b607-1fc34d0cef2c','532dada9-30ae-424b-952a-c55211373408.jpg','http://localhost:8000/imagecourses/532dada9-30ae-424b-952a-c55211373408.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('aecba5b6-fb7c-4513-8671-6a3fb77db224','53ce18f8-81d7-4e04-8286-65457dcccdd5.jpg','http://localhost:8000/imagecourses/53ce18f8-81d7-4e04-8286-65457dcccdd5.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('9ab2c8d7-caaa-4e08-8304-81e1f83751fd','540338fb-1f14-4719-b801-29a21edcb629.jpg','http://localhost:8000/imagecourses/540338fb-1f14-4719-b801-29a21edcb629.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('54ca8dbb-766f-4ac9-bc81-f4661ee05105','584c685e-6e50-4c19-a3c5-b8f46d64ba79.jpg','http://localhost:8000/imagecourses/584c685e-6e50-4c19-a3c5-b8f46d64ba79.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('53cf806d-c778-4aa0-8c9d-018920a97148','58e6f457-689d-4b14-998c-d72b8a9e8f4d.jpg','http://localhost:8000/imagecourses/58e6f457-689d-4b14-998c-d72b8a9e8f4d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('54ca8dbb-766f-4ac9-bc81-f4661ee05105','593094d7-e8e1-4c0f-ab1e-55b104b22890.jpg','http://localhost:8000/imagecourses/593094d7-e8e1-4c0f-ab1e-55b104b22890.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('9ab2c8d7-caaa-4e08-8304-81e1f83751fd','5993a713-e737-4ae2-bd13-4c9b8e52d9e7.jpg','http://localhost:8000/imagecourses/5993a713-e737-4ae2-bd13-4c9b8e52d9e7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('aecba5b6-fb7c-4513-8671-6a3fb77db224','5bdd282c-fead-4630-b961-8997d4bb054e.jpg','http://localhost:8000/imagecourses/5bdd282c-fead-4630-b961-8997d4bb054e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a6a3fad5-9979-43d1-a51d-6784146d678d','5db458d3-a896-4809-86ba-596c68e7cfac.jpg','http://localhost:8000/imagecourses/5db458d3-a896-4809-86ba-596c68e7cfac.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('3aeb3b7e-1211-4cd0-a566-6192528a1315','5de6a638-e398-4848-b05e-3eb69e3eb58a.jpg','http://localhost:8000/imagecourses/5de6a638-e398-4848-b05e-3eb69e3eb58a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('53cf806d-c778-4aa0-8c9d-018920a97148','5fb14438-41d6-49c2-bc12-ae3a20643243.jpg','http://localhost:8000/imagecourses/5fb14438-41d6-49c2-bc12-ae3a20643243.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('55dadfd9-a424-4b5e-8e01-7fd51f6c0872','60aaa248-c9cf-4459-87a2-c6cca3ec3f0b.jpg','http://localhost:8000/imagecourses/60aaa248-c9cf-4459-87a2-c6cca3ec3f0b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a4a28e63-05d9-4987-b7e5-36350c474a81','60d154b6-1b89-43ea-be85-8ac4801d2091.jpg','http://localhost:8000/imagecourses/60d154b6-1b89-43ea-be85-8ac4801d2091.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('55dadfd9-a424-4b5e-8e01-7fd51f6c0872','61c5196f-3683-4ce8-8533-f5a9701c3753.jpg','http://localhost:8000/imagecourses/61c5196f-3683-4ce8-8533-f5a9701c3753.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a4a28e63-05d9-4987-b7e5-36350c474a81','650b447c-df76-416e-90e6-23b2b692daab.jpg','http://localhost:8000/imagecourses/650b447c-df76-416e-90e6-23b2b692daab.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a21f350a-bfdd-4cbd-b3a6-98b793a2a452','6772f7c0-a8f5-4985-a3da-2259d066ff57.jpg','http://localhost:8000/imagecourses/6772f7c0-a8f5-4985-a3da-2259d066ff57.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('57f9cbd2-1447-4ed4-be70-5257650ab501','67a5fd37-7650-4a7d-b197-44a128f4f620.jpg','http://localhost:8000/imagecourses/67a5fd37-7650-4a7d-b197-44a128f4f620.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a6a3fad5-9979-43d1-a51d-6784146d678d','691e7d31-4194-4c46-bc69-4bb751822092.jpg','http://localhost:8000/imagecourses/691e7d31-4194-4c46-bc69-4bb751822092.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('605ffde1-043c-427d-acd4-1c770b7b596f','6c490f08-9da7-49f3-b9e6-338c5f940ef6.jpg','http://localhost:8000/imagecourses/6c490f08-9da7-49f3-b9e6-338c5f940ef6.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('56317663-0512-47d5-abfc-644fec756a12','6c4d38dc-c093-4140-91ae-4c11945aae48.jpg','http://localhost:8000/imagecourses/6c4d38dc-c093-4140-91ae-4c11945aae48.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('57f9cbd2-1447-4ed4-be70-5257650ab501','6db119b2-a961-42da-b8c5-90d1af16f23b.jpg','http://localhost:8000/imagecourses/6db119b2-a961-42da-b8c5-90d1af16f23b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a21f350a-bfdd-4cbd-b3a6-98b793a2a452','6f5a24e9-bdd5-4244-937d-091b73e9c954.jpg','http://localhost:8000/imagecourses/6f5a24e9-bdd5-4244-937d-091b73e9c954.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('5b7d3bfb-18e2-4ed0-bf21-75bddc36cdda','71198c3a-6aca-4dbf-a9ba-6d48d6de8d48.jpg','http://localhost:8000/imagecourses/71198c3a-6aca-4dbf-a9ba-6d48d6de8d48.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('a97c7c4f-4891-4569-874e-93141d337aca','71bf84f6-92b3-4e17-ab67-a4f5cc5007d7.jpg','http://localhost:8000/imagecourses/71bf84f6-92b3-4e17-ab67-a4f5cc5007d7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('abac0005-3a3f-4567-9f77-2ef2f46970ee','71c08236-0a77-4e3f-b0a6-afa30df6d645.jpg','http://localhost:8000/imagecourses/71c08236-0a77-4e3f-b0a6-afa30df6d645.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('596c5c73-4467-4d4f-876f-d17e7811c51c','725f8b53-fccf-421a-95f1-7a1ee89d12cf.jpg','http://localhost:8000/imagecourses/725f8b53-fccf-421a-95f1-7a1ee89d12cf.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('605ffde1-043c-427d-acd4-1c770b7b596f','73445699-a19a-446f-922b-5540c09aae3b.jpg','http://localhost:8000/imagecourses/73445699-a19a-446f-922b-5540c09aae3b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('161f53b2-8341-4e41-be29-0a3f4ead92de','763e0997-9b1f-42d2-83bd-0039906f1589.jpg','http://localhost:8000/imagecourses/763e0997-9b1f-42d2-83bd-0039906f1589.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1750fb51-54ab-49b4-9088-75f50b35a31d','76a2dd19-eaf3-4771-b008-33d8fc37db49.jpg','http://localhost:8000/imagecourses/76a2dd19-eaf3-4771-b008-33d8fc37db49.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('1750fb51-54ab-49b4-9088-75f50b35a31d','77b14f2b-7ab4-40bb-a884-96f07fd85702.jpg','http://localhost:8000/imagecourses/77b14f2b-7ab4-40bb-a884-96f07fd85702.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('161f53b2-8341-4e41-be29-0a3f4ead92de','77bd4cc1-8a41-47ba-8968-51df2bfacb1b.jpg','http://localhost:8000/imagecourses/77bd4cc1-8a41-47ba-8968-51df2bfacb1b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('16286a50-8b01-47ef-b0d0-46a46bdf522b','786c4145-0a82-4cad-acce-6e7d03dadc40.jpg','http://localhost:8000/imagecourses/786c4145-0a82-4cad-acce-6e7d03dadc40.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('6033dcfb-4cf8-4692-a32b-112611425fdf','799209fd-87bc-493f-af7f-51c8ccc7a138.jpg','http://localhost:8000/imagecourses/799209fd-87bc-493f-af7f-51c8ccc7a138.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('974fd87c-b0f5-4968-900c-621045b0d1c3','7a7ea123-cd15-4b11-8dfd-fe4dcb2ae107.jpg','http://localhost:8000/imagecourses/7a7ea123-cd15-4b11-8dfd-fe4dcb2ae107.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('66222930-e770-4995-af55-bfa5a999afdf','7c326acb-56d7-4703-b67c-2a0e7728a13f.jpg','http://localhost:8000/imagecourses/7c326acb-56d7-4703-b67c-2a0e7728a13f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('624e128d-4bd7-449d-b13a-256c84c2c1b1','7e056963-a9b1-4ea8-b16e-5fdc9d090015.jpg','http://localhost:8000/imagecourses/7e056963-a9b1-4ea8-b16e-5fdc9d090015.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('974fd87c-b0f5-4968-900c-621045b0d1c3','7eba0827-7ff1-43bc-95af-e3cf2ea086d0.jpg','http://localhost:8000/imagecourses/7eba0827-7ff1-43bc-95af-e3cf2ea086d0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('6471fa94-9112-41b5-af4e-8438475ab06f','84ddb37e-d969-42b5-84af-b9e248d17ff6.jpg','http://localhost:8000/imagecourses/84ddb37e-d969-42b5-84af-b9e248d17ff6.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('624e128d-4bd7-449d-b13a-256c84c2c1b1','8583cc76-2a09-4937-8846-744d25751a8d.jpg','http://localhost:8000/imagecourses/8583cc76-2a09-4937-8846-744d25751a8d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('6471fa94-9112-41b5-af4e-8438475ab06f','86d7e8c8-610c-44d1-84d9-61aef671731c.jpg','http://localhost:8000/imagecourses/86d7e8c8-610c-44d1-84d9-61aef671731c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('66222930-e770-4995-af55-bfa5a999afdf','880410ad-d1b8-477f-95a2-04d98e99e67f.jpg','http://localhost:8000/imagecourses/880410ad-d1b8-477f-95a2-04d98e99e67f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('970526ec-f465-42e1-9603-2f454e4e7f5c','880ea844-6447-4ac1-a9d8-2f1225354716.jpg','http://localhost:8000/imagecourses/880ea844-6447-4ac1-a9d8-2f1225354716.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('6b1e7876-493d-47b2-a7f7-4b0bc10f037d','89f142c4-da0a-4523-aab5-ac7212ace3d7.jpg','http://localhost:8000/imagecourses/89f142c4-da0a-4523-aab5-ac7212ace3d7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('68afd51e-9c42-4461-bce1-771a455ad167','8ac9ea0d-27dc-48cf-b392-9219d1d7b8ad.jpg','http://localhost:8000/imagecourses/8ac9ea0d-27dc-48cf-b392-9219d1d7b8ad.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('68afd51e-9c42-4461-bce1-771a455ad167','8aefa169-9011-4c61-9653-caed955e913f.jpg','http://localhost:8000/imagecourses/8aefa169-9011-4c61-9653-caed955e913f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('970526ec-f465-42e1-9603-2f454e4e7f5c','8af5a0d5-50dd-44f1-ba8c-78e907eb07ae.jpg','http://localhost:8000/imagecourses/8af5a0d5-50dd-44f1-ba8c-78e907eb07ae.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('672d5fac-1c7b-43dd-a52b-55c9c7b4c564','8d6a1ed3-cd4a-4349-8a68-874908c93069.jpg','http://localhost:8000/imagecourses/8d6a1ed3-cd4a-4349-8a68-874908c93069.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('672d5fac-1c7b-43dd-a52b-55c9c7b4c564','8e9c1703-833e-4990-b2e7-e1256781c6d7.jpg','http://localhost:8000/imagecourses/8e9c1703-833e-4990-b2e7-e1256781c6d7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('6b1e7876-493d-47b2-a7f7-4b0bc10f037d','8fe4d865-5bfe-4f97-971d-4463a0c13147.jpg','http://localhost:8000/imagecourses/8fe4d865-5bfe-4f97-971d-4463a0c13147.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('842e4d32-955e-418b-a8cf-95ee8b7a60f5','91516cd7-ba59-4bfa-a1df-6c5dced3a77a.jpg','http://localhost:8000/imagecourses/91516cd7-ba59-4bfa-a1df-6c5dced3a77a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('842e4d32-955e-418b-a8cf-95ee8b7a60f5','91d973c4-f6b0-4fbf-91c8-059326e642ec.jpg','http://localhost:8000/imagecourses/91d973c4-f6b0-4fbf-91c8-059326e642ec.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('7471b4ba-153f-4963-be74-ecb0707ecf2a','947d2670-d2db-419b-89ae-5c978af533fd.jpg','http://localhost:8000/imagecourses/947d2670-d2db-419b-89ae-5c978af533fd.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('7ea09a72-fda9-45c1-a82e-c706e509f360','956874c3-92c3-4ba8-85f6-61def20ed852.jpg','http://localhost:8000/imagecourses/956874c3-92c3-4ba8-85f6-61def20ed852.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('71a749fd-8046-4848-a1ee-2fd44ca3ceff','98c4e4b0-e696-41c0-b3de-269dec521524.jpg','http://localhost:8000/imagecourses/98c4e4b0-e696-41c0-b3de-269dec521524.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('71b1d49e-5671-43b9-80fd-546f3ab11c8d','9b47485a-5db1-4518-b38f-dc1db7c6ce24.jpg','http://localhost:8000/imagecourses/9b47485a-5db1-4518-b38f-dc1db7c6ce24.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('7a7cf7f5-1f88-4ea3-9986-7d0db7c5f928','9b660930-0383-4a0a-9c32-7628d0e752c5.jpg','http://localhost:8000/imagecourses/9b660930-0383-4a0a-9c32-7628d0e752c5.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('8f899036-73a1-41ec-a0c8-8d9ed2e3f1ef','9b6b6926-fde2-4b5e-908f-3783c52ab7ff.jpg','http://localhost:8000/imagecourses/9b6b6926-fde2-4b5e-908f-3783c52ab7ff.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('712a97a8-6fa1-4112-a3bb-f5faff06a27b','9cd705f1-30bb-44c6-9cc3-a78fcbfc4ca7.jpg','http://localhost:8000/imagecourses/9cd705f1-30bb-44c6-9cc3-a78fcbfc4ca7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('72402f43-0550-4f17-aa70-9640eaaf7434','9f20c96b-2681-4513-a388-7d040a1f206d.jpg','http://localhost:8000/imagecourses/9f20c96b-2681-4513-a388-7d040a1f206d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('86b7c89c-a188-4628-a846-5636c1b28698','9f454104-f199-4df6-baa7-cc133407e48b.jpg','http://localhost:8000/imagecourses/9f454104-f199-4df6-baa7-cc133407e48b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('8bc57e07-7df4-4eaa-80e6-331a808c07f6','9fbcdbd0-4be1-4f6e-9e86-146c81415f41.jpg','http://localhost:8000/imagecourses/9fbcdbd0-4be1-4f6e-9e86-146c81415f41.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('7d49b5ff-8803-4f4d-aff7-783d6d77b1f4','a05672f6-f45b-4165-bbb0-e7e7f525e570.jpg','http://localhost:8000/imagecourses/a05672f6-f45b-4165-bbb0-e7e7f525e570.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('904db638-7bc3-481a-8fd6-1657862cf8ec','a22f46dd-bd15-4364-a50b-9062f57af3bf.jpg','http://localhost:8000/imagecourses/a22f46dd-bd15-4364-a50b-9062f57af3bf.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('9025bd06-7199-4026-b603-750f96724553','a240c414-bfb3-40ad-99a6-ab26ebd95ea1.jpg','http://localhost:8000/imagecourses/a240c414-bfb3-40ad-99a6-ab26ebd95ea1.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('8f761dc1-d6eb-4ad9-9326-f88a181cdb93','a241ab3a-bbb3-4553-b6ef-a27355aa42d3.jpg','http://localhost:8000/imagecourses/a241ab3a-bbb3-4553-b6ef-a27355aa42d3.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('751dd016-4b9b-4cfc-b5ab-1b644566d830','a2af16ab-9da0-4cd5-bf53-03f8d3a511e1.jpg','http://localhost:8000/imagecourses/a2af16ab-9da0-4cd5-bf53-03f8d3a511e1.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('8c8ba9a3-b839-4c5e-be3d-bcc79e8c1fcd','a5fc5a55-2d54-4f7d-81dd-64b2b0248920.jpg','http://localhost:8000/imagecourses/a5fc5a55-2d54-4f7d-81dd-64b2b0248920.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('96d014df-f0af-4b4c-957c-3eb823bdc6ab','a7447c4f-9535-4431-83fd-5e75eb9a8717.jpg','http://localhost:8000/imagecourses/a7447c4f-9535-4431-83fd-5e75eb9a8717.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ac385087-ff3b-42a4-af39-12e67bd8ee0c','a8d3bf50-313d-4da8-a89b-76bfd1bee489.jpg','http://localhost:8000/imagecourses/a8d3bf50-313d-4da8-a89b-76bfd1bee489.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ac71448f-15b5-4913-84be-8310cc1f6aef','a9177765-8495-46f9-8fd2-ad213ef4835c.jpg','http://localhost:8000/imagecourses/a9177765-8495-46f9-8fd2-ad213ef4835c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b3ba9924-7cfc-4721-a616-f61585ac4f78','a97ea003-8e78-4974-adaf-ac1f4fa6e274.jpg','http://localhost:8000/imagecourses/a97ea003-8e78-4974-adaf-ac1f4fa6e274.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ade4ddd0-943c-43b1-825a-76414ed75d3d','aa6f09e0-ac3f-4729-9a96-379259290c43.jpg','http://localhost:8000/imagecourses/aa6f09e0-ac3f-4729-9a96-379259290c43.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c0627319-216b-4740-afb4-ec2ccf480a2f','ab3c3e57-ea48-4602-a6a4-7af639b62a08.jpg','http://localhost:8000/imagecourses/ab3c3e57-ea48-4602-a6a4-7af639b62a08.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b6711096-cf8e-48f7-8a7b-97f5900ed754','ab87d574-150a-445d-9d05-316603e76226.jpg','http://localhost:8000/imagecourses/ab87d574-150a-445d-9d05-316603e76226.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b8315209-e30a-4268-b8f4-136c7d821f8e','ad42f018-aae7-41f5-8d55-74b9780a2b54.jpg','http://localhost:8000/imagecourses/ad42f018-aae7-41f5-8d55-74b9780a2b54.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b8315209-e30a-4268-b8f4-136c7d821f8e','af1550c9-f212-4b5b-97f9-683400a09fa7.jpg','http://localhost:8000/imagecourses/af1550c9-f212-4b5b-97f9-683400a09fa7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('abdb23a3-e949-471e-8830-cac30d4af60a','af2b6cfc-4367-4b79-9cb4-377f59ea92c3.jpg','http://localhost:8000/imagecourses/af2b6cfc-4367-4b79-9cb4-377f59ea92c3.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b5733f63-50b0-4b27-a6d3-f80f8be9e1f8','b09fac3c-458e-4fbc-8aae-bf4f52092b5c.jpg','http://localhost:8000/imagecourses/b09fac3c-458e-4fbc-8aae-bf4f52092b5c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ceae4aa2-22ab-4542-9190-39f5fbc39674','b2986453-9b98-486a-9c48-85d78af18607.jpg','http://localhost:8000/imagecourses/b2986453-9b98-486a-9c48-85d78af18607.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b54ebea7-d2af-4811-b337-77fdf7e29ddf','b3095b65-09b3-4546-8923-23f5fb499e1b.jpg','http://localhost:8000/imagecourses/b3095b65-09b3-4546-8923-23f5fb499e1b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c3051fcf-9ae8-42dc-8ebb-735cad1a6ac3','b39da918-e174-45cd-a6d2-9176faf3b673.jpg','http://localhost:8000/imagecourses/b39da918-e174-45cd-a6d2-9176faf3b673.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ceae4aa2-22ab-4542-9190-39f5fbc39674','b4518016-66df-4c44-bc10-25cd01a0e2ae.jpg','http://localhost:8000/imagecourses/b4518016-66df-4c44-bc10-25cd01a0e2ae.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('b4302f9b-7c0f-4c71-b655-c06686e2ec2f','b487e65e-b7b8-4595-bc18-8a45fd1d2168.jpg','http://localhost:8000/imagecourses/b487e65e-b7b8-4595-bc18-8a45fd1d2168.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c1c00896-60fa-430e-999f-5d94780014e7','b52d117c-03db-4f2e-8b99-6e61e82562f2.jpg','http://localhost:8000/imagecourses/b52d117c-03db-4f2e-8b99-6e61e82562f2.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c201e7dc-8b69-4bc1-8fc2-2d588f2b2829','b5781fda-249d-4527-b632-254ef1681eda.jpg','http://localhost:8000/imagecourses/b5781fda-249d-4527-b632-254ef1681eda.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d0b28e85-fab3-4f0e-b952-c0454443c496','b9d4b376-b452-4994-ad2d-f53f9999f04b.jpg','http://localhost:8000/imagecourses/b9d4b376-b452-4994-ad2d-f53f9999f04b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c201e7dc-8b69-4bc1-8fc2-2d588f2b2829','bbefd281-94ba-48b1-8b3e-e6b892ed0766.jpg','http://localhost:8000/imagecourses/bbefd281-94ba-48b1-8b3e-e6b892ed0766.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('bda2b774-d1a1-4690-83ef-f5cc1bfe1f2f','bd404c82-7b36-46e9-b497-d754137b181d.jpg','http://localhost:8000/imagecourses/bd404c82-7b36-46e9-b497-d754137b181d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c1c00896-60fa-430e-999f-5d94780014e7','bda3140d-2ece-4cb0-ae72-d956676f2b2b.jpg','http://localhost:8000/imagecourses/bda3140d-2ece-4cb0-ae72-d956676f2b2b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ca5163c4-781a-485e-a614-4bf6ecf2c99e','be1aecf6-2029-4659-ad7f-0ab8f6a1d1c0.jpg','http://localhost:8000/imagecourses/be1aecf6-2029-4659-ad7f-0ab8f6a1d1c0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('c8ee397f-4eb0-42e3-82ed-24e95a61b355','be30762a-565b-4044-ab2f-323d0c1d3832.jpg','http://localhost:8000/imagecourses/be30762a-565b-4044-ab2f-323d0c1d3832.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ceb61570-68ff-46b1-91b5-2753da334058','c055032b-6c98-4bf1-893b-09c66b77da07.jpg','http://localhost:8000/imagecourses/c055032b-6c98-4bf1-893b-09c66b77da07.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d28f314b-1fab-423e-bd22-734dfbf93b7e','c1121ecf-af20-4d6b-951d-f2211dc39535.jpg','http://localhost:8000/imagecourses/c1121ecf-af20-4d6b-951d-f2211dc39535.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d28f314b-1fab-423e-bd22-734dfbf93b7e','c281c84f-bd6d-4ec2-ac58-b4a76dff180e.jpg','http://localhost:8000/imagecourses/c281c84f-bd6d-4ec2-ac58-b4a76dff180e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d55bd31b-57bb-4186-b4a0-55a84f772e17','c379b9e3-056a-481f-b627-e0ce2c8e6572.jpg','http://localhost:8000/imagecourses/c379b9e3-056a-481f-b627-e0ce2c8e6572.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d51808db-ae01-49c4-97dc-b75db10cdddb','c38d402e-73e6-43ac-8ff1-3ffcc0f863ba.jpg','http://localhost:8000/imagecourses/c38d402e-73e6-43ac-8ff1-3ffcc0f863ba.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d8b66fe0-e20f-4a1f-a76a-5f9e59239bc3','c45349ca-6c30-4d81-b291-0ca148988fa9.jpg','http://localhost:8000/imagecourses/c45349ca-6c30-4d81-b291-0ca148988fa9.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d51808db-ae01-49c4-97dc-b75db10cdddb','c4b4718a-3d53-42fb-a727-56ccd420f9a9.jpg','http://localhost:8000/imagecourses/c4b4718a-3d53-42fb-a727-56ccd420f9a9.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d55bd31b-57bb-4186-b4a0-55a84f772e17','c774fbc7-3d18-47bc-b0fc-0b31d4f312b0.jpg','http://localhost:8000/imagecourses/c774fbc7-3d18-47bc-b0fc-0b31d4f312b0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d89568f8-ed52-4557-8ee1-766de9df0a72','ca747f88-de2e-48b7-a6e5-9026650e36ea.jpg','http://localhost:8000/imagecourses/ca747f88-de2e-48b7-a6e5-9026650e36ea.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('cc621e12-4628-46f5-9d33-77420f589659','caa1a463-9f92-4557-9bfe-8596ac6387ad.jpg','http://localhost:8000/imagecourses/caa1a463-9f92-4557-9bfe-8596ac6387ad.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d89568f8-ed52-4557-8ee1-766de9df0a72','cbb41a63-b1b5-47c4-a22e-02b097dc743d.jpg','http://localhost:8000/imagecourses/cbb41a63-b1b5-47c4-a22e-02b097dc743d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d0f239f5-0d23-44b6-b5a7-74d6595d24d7','cd8cb37e-c5c8-4c27-ad27-521b1912fc6f.jpg','http://localhost:8000/imagecourses/cd8cb37e-c5c8-4c27-ad27-521b1912fc6f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('de3d8846-44bc-4589-94ef-145fd6086b25','cd8fb4a7-cf28-4d3c-8eef-c7880c92c33b.jpg','http://localhost:8000/imagecourses/cd8fb4a7-cf28-4d3c-8eef-c7880c92c33b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('de3d8846-44bc-4589-94ef-145fd6086b25','ce8f5ba2-3184-478a-a262-720c0d818adc.jpg','http://localhost:8000/imagecourses/ce8f5ba2-3184-478a-a262-720c0d818adc.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e2afa08d-1cdc-43e2-ad83-582356a4f5da','cec5b37c-2066-46f1-99a7-4a1acdc79e03.jpg','http://localhost:8000/imagecourses/cec5b37c-2066-46f1-99a7-4a1acdc79e03.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d0f239f5-0d23-44b6-b5a7-74d6595d24d7','cf39a30e-3ca9-47f7-a1a9-06544e4ef4ad.jpg','http://localhost:8000/imagecourses/cf39a30e-3ca9-47f7-a1a9-06544e4ef4ad.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('deb16b8d-966c-4f2b-9305-e454cb449efd','d039ca01-e319-4cdc-8a67-c7bd34f6d2a6.jpg','http://localhost:8000/imagecourses/d039ca01-e319-4cdc-8a67-c7bd34f6d2a6.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e2afa08d-1cdc-43e2-ad83-582356a4f5da','d1f334ce-106a-430d-b595-9494e914edad.jpg','http://localhost:8000/imagecourses/d1f334ce-106a-430d-b595-9494e914edad.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e2d12459-d1da-414f-9277-4f6b6327fff1','d3e1bb4c-a3b8-4992-8c98-f9e00d57bf71.jpg','http://localhost:8000/imagecourses/d3e1bb4c-a3b8-4992-8c98-f9e00d57bf71.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e305ff28-576e-472f-a6c6-5fe4c89b3d48','d5f9b3c4-99a7-483c-be36-37dc52ba5e20.jpg','http://localhost:8000/imagecourses/d5f9b3c4-99a7-483c-be36-37dc52ba5e20.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e305ff28-576e-472f-a6c6-5fe4c89b3d48','d63f8aa0-6d91-41bb-8850-d5496492beae.jpg','http://localhost:8000/imagecourses/d63f8aa0-6d91-41bb-8850-d5496492beae.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('cc621e12-4628-46f5-9d33-77420f589659','d670c0a7-c228-42cb-8af3-517e5cbd169f.jpg','http://localhost:8000/imagecourses/d670c0a7-c228-42cb-8af3-517e5cbd169f.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('d8b66fe0-e20f-4a1f-a76a-5f9e59239bc3','d804dee5-dffb-423c-8f9c-fb19607d06bb.jpg','http://localhost:8000/imagecourses/d804dee5-dffb-423c-8f9c-fb19607d06bb.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e30ac57d-51a8-47bf-a9c0-4ec7cfae20da','da21f88c-c1bf-4fde-8b1d-bf2dafad1123.jpg','http://localhost:8000/imagecourses/da21f88c-c1bf-4fde-8b1d-bf2dafad1123.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e43d0c5c-a21e-4fdb-b8af-44ca6206e87b','dad17762-af9f-458f-b5f9-c555063abcf4.jpg','http://localhost:8000/imagecourses/dad17762-af9f-458f-b5f9-c555063abcf4.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ce3cb7fc-ec00-4271-af7b-8168ec1d03ec','dbf85ec5-08f8-4019-a332-f20b777c7886.jpg','http://localhost:8000/imagecourses/dbf85ec5-08f8-4019-a332-f20b777c7886.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('dc74d51d-9b27-4513-845e-be51961068d2','df30a0bd-54f3-4a4c-962b-3375b903e1bc.jpg','http://localhost:8000/imagecourses/df30a0bd-54f3-4a4c-962b-3375b903e1bc.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('deb16b8d-966c-4f2b-9305-e454cb449efd','df936eeb-4704-4ff5-833b-c9f628fb6585.jpg','http://localhost:8000/imagecourses/df936eeb-4704-4ff5-833b-c9f628fb6585.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e30ac57d-51a8-47bf-a9c0-4ec7cfae20da','dfe790a7-006b-411a-9f04-c344cdd6c385.jpg','http://localhost:8000/imagecourses/dfe790a7-006b-411a-9f04-c344cdd6c385.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e2d12459-d1da-414f-9277-4f6b6327fff1','e4b64b8f-7e5d-4de4-9827-89cfaa99b3f6.jpg','http://localhost:8000/imagecourses/e4b64b8f-7e5d-4de4-9827-89cfaa99b3f6.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e43d0c5c-a21e-4fdb-b8af-44ca6206e87b','e6c99b88-81c0-456a-a83b-407ac4296210.jpg','http://localhost:8000/imagecourses/e6c99b88-81c0-456a-a83b-407ac4296210.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ce3cb7fc-ec00-4271-af7b-8168ec1d03ec','e79fbbc6-efd4-4011-85b5-6157c357e5ee.jpg','http://localhost:8000/imagecourses/e79fbbc6-efd4-4011-85b5-6157c357e5ee.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e70f14d1-abf9-48b6-b9a1-7b88cd75a452','e7ff1756-e488-4ee8-9c4a-8de6cd802350.jpg','http://localhost:8000/imagecourses/e7ff1756-e488-4ee8-9c4a-8de6cd802350.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('dc74d51d-9b27-4513-845e-be51961068d2','e8b6c756-b44f-40b8-89fa-ad4ff91d062e.jpg','http://localhost:8000/imagecourses/e8b6c756-b44f-40b8-89fa-ad4ff91d062e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e157a879-8da8-49ef-933d-1b075909f242','e8c6b76f-83df-432f-b981-06942e0d1cf2.jpg','http://localhost:8000/imagecourses/e8c6b76f-83df-432f-b981-06942e0d1cf2.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e4add69d-70e1-4f2e-bfe1-351ce70313c8','eaa4062b-e70b-40ec-9fa2-5728b406f65d.jpg','http://localhost:8000/imagecourses/eaa4062b-e70b-40ec-9fa2-5728b406f65d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e995b209-cff3-4f42-be24-dfb58b8ab91d','ec67809b-6736-448c-8682-cba0866a411e.jpg','http://localhost:8000/imagecourses/ec67809b-6736-448c-8682-cba0866a411e.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e995b209-cff3-4f42-be24-dfb58b8ab91d','ee86833f-0b92-4c9d-aac8-d2a9a29ecb78.jpg','http://localhost:8000/imagecourses/ee86833f-0b92-4c9d-aac8-d2a9a29ecb78.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e82bd4ac-a1cd-49dc-88be-c574addf0534','ee90ec38-0f77-4327-b44d-b88233620bba.jpg','http://localhost:8000/imagecourses/ee90ec38-0f77-4327-b44d-b88233620bba.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ed1773ed-6084-4b72-9fab-1b9dfb58e850','eeb6690c-a3c0-4d73-8cb9-32be06f750df.jpg','http://localhost:8000/imagecourses/eeb6690c-a3c0-4d73-8cb9-32be06f750df.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e157a879-8da8-49ef-933d-1b075909f242','ef38c33d-3f95-46c4-a54f-6236eca0f267.jpg','http://localhost:8000/imagecourses/ef38c33d-3f95-46c4-a54f-6236eca0f267.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e82bd4ac-a1cd-49dc-88be-c574addf0534','f00d186a-93ca-4131-b2c6-acaf3c614e92.jpg','http://localhost:8000/imagecourses/f00d186a-93ca-4131-b2c6-acaf3c614e92.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ed1773ed-6084-4b72-9fab-1b9dfb58e850','f0597e70-d8b0-4002-b34f-424f27ef721d.jpg','http://localhost:8000/imagecourses/f0597e70-d8b0-4002-b34f-424f27ef721d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ee4089b9-4963-47bb-b78a-33575db40b06','f19c1602-db5a-417a-b832-1c61ae54550d.jpg','http://localhost:8000/imagecourses/f19c1602-db5a-417a-b832-1c61ae54550d.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('ee4089b9-4963-47bb-b78a-33575db40b06','f2322f3a-5d8b-41cd-8250-30ffadd38380.jpg','http://localhost:8000/imagecourses/f2322f3a-5d8b-41cd-8250-30ffadd38380.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e4add69d-70e1-4f2e-bfe1-351ce70313c8','f2c6ae9d-ab4c-40e4-836e-ac2b748ebc23.jpg','http://localhost:8000/imagecourses/f2c6ae9d-ab4c-40e4-836e-ac2b748ebc23.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('f86b65b4-f71f-4b67-a374-5a30f6dfbbc8','f319bb4f-adb8-450f-8371-a8bdbe7c22e7.jpg','http://localhost:8000/imagecourses/f319bb4f-adb8-450f-8371-a8bdbe7c22e7.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e750bd85-2e87-4ac4-a000-cc228973b885','f4dce8d6-711e-4035-ade4-dc24f445070c.jpg','http://localhost:8000/imagecourses/f4dce8d6-711e-4035-ade4-dc24f445070c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('f86b65b4-f71f-4b67-a374-5a30f6dfbbc8','f4eedfa1-e831-4604-ad42-36cd8cf90a4a.jpg','http://localhost:8000/imagecourses/f4eedfa1-e831-4604-ad42-36cd8cf90a4a.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fb15c268-c397-4794-ad39-22a2e592a7c1','f63f799c-629a-4d6a-b0b7-b4ee07431595.jpg','http://localhost:8000/imagecourses/f63f799c-629a-4d6a-b0b7-b4ee07431595.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fe45080f-94c6-4570-96db-27ad3cdfe782','f64dc646-061d-4b0a-a2a3-a757378c151c.jpg','http://localhost:8000/imagecourses/f64dc646-061d-4b0a-a2a3-a757378c151c.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('e750bd85-2e87-4ac4-a000-cc228973b885','f71a378f-28af-4cd1-abae-fef7d9bb5df2.jpg','http://localhost:8000/imagecourses/f71a378f-28af-4cd1-abae-fef7d9bb5df2.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fe45080f-94c6-4570-96db-27ad3cdfe782','f8480daa-3b05-4ae9-a08b-e00ea7ad59a0.jpg','http://localhost:8000/imagecourses/f8480daa-3b05-4ae9-a08b-e00ea7ad59a0.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fb15c268-c397-4794-ad39-22a2e592a7c1','fb836629-3fe5-4ba9-8fd1-31d8c79752f4.jpg','http://localhost:8000/imagecourses/fb836629-3fe5-4ba9-8fd1-31d8c79752f4.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fa3180d6-5fc0-4430-ad3d-ed644ca4dafe','fbe7393e-7257-4a21-900c-93904e65b755.jpg','http://localhost:8000/imagecourses/fbe7393e-7257-4a21-900c-93904e65b755.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fc995911-7ae2-4647-9641-7b64c1479f24','fd99b07f-7292-4ad2-9fde-8a56ad535be2.jpg','http://localhost:8000/imagecourses/fd99b07f-7292-4ad2-9fde-8a56ad535be2.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fc995911-7ae2-4647-9641-7b64c1479f24','feaae677-1d75-46b4-a78f-befa76b286be.jpg','http://localhost:8000/imagecourses/feaae677-1d75-46b4-a78f-befa76b286be.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fa3180d6-5fc0-4430-ad3d-ed644ca4dafe','ff3b5c9b-c6ad-4e8d-924c-5296c828916b.jpg','http://localhost:8000/imagecourses/ff3b5c9b-c6ad-4e8d-924c-5296c828916b.jpg','IMAGE')
Insert into FileCourses (CourseId, ObjectId, Url, TypeFile) Values ('fc995911-7ae2-4647-9641-7b64c1479f24','ff9056ad-87ae-4922-8546-a2047700222b.jpg','http://localhost:8000/imagecourses/ff9056ad-87ae-4922-8546-a2047700222b.jpg','IMAGE')

insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream cheese', 51, 5, 0.5, 0.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('neufchatel cheese', 215, 19.4, 2.7, 7.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('requeijao cremoso light catupiry', 49, 3.6, 3.4, 0.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ricotta cheese', 30, 2, 0.091, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream cheese low fat', 30, 2.3, 0.9, 1.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream cheese fat free', 19, 0.2, 1, 2.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gruyere cheese', 116, 9.1, 0.1, 8.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheddar cheese', 113, 9.3, 0.1, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('parmesan cheese', 71, 4.5, 0.046, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('romano cheese', 19, 1.3, 0.088, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('parmesan cheese grated', 21, 1.4, 0.075, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('port salut cheese', 465, 37.2, 0.8, 31.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('swiss cheese', 98, 7.7, 0, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('goat cheese hard', 128, 10.1, 0.6, 8.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gouda cheese', 100, 7.7, 0.6, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pepper jack cheese lucerne', 75, 6, 0, 5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caraway cheese', 106, 8.3, 0, 7.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gjetost cheese', 1058, 67, 0, 21.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tilsit cheese', 136, 10.4, 0, 9.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('goat cheese', 103, 8.4, 0.031, 6.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brick cheese', 111, 8.9, 0.2, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('asadero cheese', 402, 31.9, 3.2, 25.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('camembert cheese', 90, 7.3, 0.1, 5.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('provolone cheese reduced fat', 310, 19.9, 0.6, 27.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roquefort cheese', 314, 26, 0, 18.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('queso blanco cheese', 366, 28.7, 2.1, 24) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('queso seco cheese', 315, 23.6, 0.5, 23.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('goat cheese soft', 75, 6, 0.3, 5.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mozzarella cheese', 90, 6.6, 0, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chihuahua cheese', 494, 39.2, 7.3, 28.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('limburger cheese', 98, 8.2, 0.1, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('muenster cheese', 486, 39.7, 1.5, 30.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('queso fresco cheese', 365, 29.1, 2.8, 22.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brie cheese', 100, 8.3, 0.1, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pimento cheese', 525, 43.7, 0.9, 31) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mexican cheese', 316, 21.7, 0.6, 27.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('feta cheese', 80, 6.4, 0, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mozzarella cheese fat free', 159, 0, 1.7, 35.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('provolone cheese', 463, 35.1, 0.7, 33.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('anejo cheese', 492, 39.6, 6.1, 28.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('honey', 64, 0, 17.2, 0.080) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apple butter', 35, 0.063, 7.1, 0.045) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit jam', 56, 0.095, 9.7, 0.000) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate hazelnut spread', 100, 5.5, 10, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut butter', 88, 7.4, 1, 3.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut spread', 101, 8.5, 0.5, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken spread', 88, 9.8, 0.3, 10.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese spread', 708, 68.6, 8.4, 17) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tahini', 86, 7.2, 0, 2.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('orange marmalade', 49, 0, 12, 0.014) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('american cheese spread', 46, 3.4, 1.2, 2.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apricot jam', 48, 0.010, 8.7, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chunky peanut butter', 94, 8, 1.3, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ham and cheese spread', 37, 2.8, 0, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken and rice casserole homade', 27, 1, 0, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('picnic loaf', 65, 4.7, 0, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn tamale', 309, 12, 12.3, 5.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baked potato with cheese sauce bacon', 451, 25.9, 0, 18.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chinese egg roll', 223, 10.6, 0, 7.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('butter croissant', 231, 12, 6.4, 4.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('enchilada with cheese beef', 323, 17.6, 0, 11.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corned beef hash with potatoes', 387, 24.2, 0.8, 20.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg cheese bacon', 436, 25.3, 2.5, 17.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bagel with ham egg cheese', 483, 18.5, 7, 26.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('quesadilla with chicken', 529, 27.5, 3.4, 27.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn rice', 110, 0.3, 3, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('kung pao chicken', 779, 42.2, 18.3, 59) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef empanada', 298, 16.3, 1.6, 10.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('frijoles with cheese', 225, 7.8, 0, 11.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burrito with beef', 285, 8.1, 4.1, 9.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken sandwich', 350, 17.6, 3.9, 14.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('enchilada with cheese', 374, 25.3, 3.6, 15.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pork egg roll', 189, 6.1, 4.5, 8.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn on the cob with butter', 155, 3.4, 0, 4.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pancakes with butter syrup', 260, 7, 0, 4.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burrito with beans', 224, 6.7, 0, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scrambled eggs', 100, 7.6, 0.8, 6.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baked potato with sour cream', 393, 22.3, 0, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey and gravy', 161, 6.3, 0, 14.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken egg roll', 158, 3.6, 4.5, 8.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable egg roll', 153, 3.6, 5.3, 5.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ham cheese roll', 67, 5.2, 0, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with ham', 386, 18.4, 2.2, 13.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lemon chicken', 1440, 74.9, 55.2, 68.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hummus', 435, 21.1, 0.7, 12) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('croissant with egg cheese', 368, 24.7, 0, 12.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('enchirito with cheese beef beans', 344, 16.1, 0, 17.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sweet sour pork', 38, 2.2, 1.4, 1.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crab cake', 160, 10.4, 0, 11.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg dinner roll', 107, 2.2, 1.5, 3.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('english muffin with egg cheese sausage', 472, 29.9, 2.5, 22.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('buttermilk pancakes', 86, 3.5, 0, 2.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('omelet', 94, 7.1, 0.2, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('taco with chicken cheese lettuce', 185, 6.2, 1.3, 13) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spinach souffle', 230, 17.6, 2.5, 10.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('english muffin with cheese sausage', 365, 22.3, 2.1, 14.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('potato gratin', 328, 18.6, 0, 12.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg', 436, 25.3, 2.5, 17.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('french fries deep fried', 222, 10.5, 0.2, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sweet sour chicken', 46, 2.3, 2.1, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burrito with beans beef', 460, 18, 5.3, 27.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nachos with cinnamon sugar', 592, 36, 0, 7.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('arroz con grandules', 209, 5.7, 0, 4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chili con carne', 271, 8.8, 4.7, 14.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('arroz con abichuelas', 223, 5.4, 0.2, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with sausage', 412, 27.1, 1.8, 10.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetarian stew', 304, 7.4, 3.1, 42) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken sandwich with cheese', 632, 38.8, 0, 29.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg steak', 410, 28.4, 0, 17.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('taco salad', 212, 11.2, 0, 10) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('succotash', 213, 1.5, 0, 9.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nachos with cheese jalapeno peppers', 608, 34.1, 0, 16.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg ham', 442, 27, 4.2, 20.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ladyfingers', 103, 2.6, 7.2, 3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mozzarella steak fried', 102, 5.9, 0.7, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tamale navajo', 285, 11.4, 1.8, 11.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blueberry pancakes', 84, 3.5, 0, 2.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chimichanga with beef cheese', 443, 23.4, 0, 20.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg cheese sandwich', 340, 19.4, 0, 15.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg bacon', 458, 31.1, 3.3, 17) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pupusas con queso', 300, 15.5, 1.4, 13.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apple croissant', 145, 5, 0, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('refried red beans', 336, 16.1, 0, 11.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tostada with guacamole', 180, 11.6, 0, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken sandwich', 468, 20.9, 6.8, 30.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fried rice', 228, 3.2, 0.6, 6.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pupusas del cerdo', 283, 12.7, 1.8, 14) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey pot pie', 699, 34.9, 0, 25.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese croissant', 331, 16.7, 9.1, 7.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chimichanga with beef', 425, 19.7, 0, 19.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('french toast with butter', 178, 9.4, 0, 5.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ham egg cheese sandwich', 347, 16.3, 0, 19.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('english muffin with butter', 189, 5.8, 0, 4.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('savoury noodle one pan dinner tandaco', 172, 1.2, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ham cheese sandwich', 352, 15.5, 0, 20.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sweet gefilte fish', 35, 0.7, 0, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('poached egg', 72, 4.7, 0.2, 6.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tostada with beef cheese', 315, 16.3, 0, 19) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tostada with beans cheese', 223, 9.9, 0, 9.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pork tamale', 247, 12.8, 0.7, 10.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nachos with cheese', 274, 17.2, 1.7, 3.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pulled pork in barbecue sauce', 418, 11, 37.8, 32.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baked potato with cheese sauce', 474, 28.7, 0, 14.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baked beans', 310, 10.3, 0, 11.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('arroz con frijoles', 220, 5.6, 1.3, 6.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('taco with beef cheese lettuce', 156, 8.8, 0.6, 6.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burrito with beans cheese beef', 165, 6.6, 0, 7.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg mix', 30, 2.2, 0.091, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('falafel', 57, 3, 0, 2.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('taco salad with chili con carne', 193, 8.8, 0, 11.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wonton wrappers', 93, 0.5, 0, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hushpuppies', 512, 20.5, 3.1, 11.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken pot pie', 598, 34.6, 7.2, 14.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('general tsos chicken', 1578, 87.5, 62.1, 69) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burrito with beans cheese', 379, 11.2, 3.2, 13.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetarian fillets', 247, 15.3, 0.7, 19.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit with egg sausage', 505, 33.6, 1.5, 18) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken chow mein', 513, 16.9, 10.5, 40.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit burrito', 231, 9.5, 0, 2.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('arroz con leche', 369, 9.3, 38.6, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caramel custard flan', 444, 12.3, 70.8, 13.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gelatin dessert', 167, 0, 36.4, 3.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate milk dessert', 229, 1.4, 51.6, 5.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate mousse', 338, 24, 22.2, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dulce de leche', 60, 1.4, 9.5, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apple crisp', 454, 9.7, 55.5, 4.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spinach spaghetti cooked', 182, 0.9, 0, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn pasta cooked', 176, 1, 0, 3.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spaghetti with meat sauce', 255, 2.9, 7.4, 14.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pasta with meatballs in tomato sauce', 273, 13, 7.2, 10.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese tortellini', 332, 7.8, 1, 14.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pasta with sliced franks in tomato sauce', 227, 6, 8, 11) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lasagna', 166, 6.1, 3.8, 9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('macaroni cheese', 310, 9.4, 3, 12.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable lasagna', 316, 13.7, 5.8, 15.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spinach egg noodles cooked', 211, 2.5, 1.1, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese lasagna', 316, 13, 10.3, 15.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pasta with tomato sauce', 176, 1.8, 10.1, 5.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spinach pasta cooked', 37, 0.3, 0, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable chicken soup', 166, 4.8, 3.4, 12.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bean ham soup', 198, 2.5, 7.8, 10.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of chicken soup', 227, 14.5, 1.4, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato soup canned', 139, 3.3, 16.5, 6.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg drop soup', 65, 1.5, 0.2, 2.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scotch broth', 197, 6.4, 0, 12.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of onion soup', 268, 12.8, 11.1, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken broth dry', 10, 0.2, 0, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable soup with beef broth', 162, 3.8, 4, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chili with beans canned', 287, 14.1, 3, 14.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef noodle soup', 168, 6.2, 5.2, 9.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('consomme dry', 17, 0.2, 1.1, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken mushroom soup', 126, 6.1, 1, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef vegetable soup', 120, 2.9, 3.1, 8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('black bean soup', 234, 3.4, 6.4, 12.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('split pea soup with ham bacon', 189, 3.1, 0, 11.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of mushroom soup', 199, 13.4, 1, 3.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('green pea soup', 320, 5.7, 16.7, 16.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken broth soup', 78, 2.6, 0.9, 11.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken mushroom chowder soup', 431, 23.7, 0, 16.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken dumplings soup', 235, 13.4, 1.4, 13.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken vegetable soup', 148, 5.6, 3, 7.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey vegetable soup', 72, 3, 2.6, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wonton soup', 71, 0.6, 0.8, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef stock', 31, 0.2, 1.3, 4.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mushroom barley soup', 186, 5.5, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gazpacho soup', 70, 0.4, 2.2, 10.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef mushroom soup', 186, 7.3, 0, 14) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken rice soup', 127, 3.2, 1.4, 12.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pepperpot soup', 100, 4.5, 0, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable soup', 149, 4, 7.9, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chili beef soup', 308, 6.7, 13.4, 13.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken soup', 174, 6.5, 2.1, 12.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bean with frankfurters soup', 188, 7, 0, 10) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pea soup', 161, 2.8, 8.3, 8.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey soup', 135, 4.4, 0, 10.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('stockpot soup', 201, 7.8, 0, 9.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hot sour soup', 91, 2.8, 1, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese soup', 203, 9.7, 9.3, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of potato soup', 186, 4.7, 3.6, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bean with bacon soup', 106, 2.1, 0.6, 5.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish stock', 40, 1.9, 0, 5.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('split pea soup with ham', 381, 8.9, 0, 20.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef broth soup', 17, 0.5, 0, 2.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lentil soup with ham', 139, 2.8, 0, 9.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato soup', 98, 0.7, 12.2, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish broth', 39, 1.5, 0.2, 4.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chili without beans canned', 283, 17, 0, 18.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('minestrone soup', 167, 5, 3.7, 8.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('clam chowder soup', 154, 4.4, 6.8, 4.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato vegetable soup', 54, 0.8, 2.4, 1.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of rice coup', 127, 0.2, 0.022, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of celery soup', 181, 11.2, 3.4, 3.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken gravy', 188, 13.6, 1.9, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato rice soup', 240, 5.5, 15.2, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vegetable beef soup', 128, 2, 0, 18.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato beef noodle soup', 281, 8.6, 0, 8.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oyster stew soup', 118, 7.7, 0, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ramen noodle soup dry', 371, 13.3, 1.3, 8.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken noodle soup', 145, 4.7, 0, 7.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken bouillon dry', 11, 0.6, 0.7, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of asparagus soup', 174, 8.2, 1.8, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey noodle soup', 139, 4, 1, 7.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of shrimp soup', 181, 10.4, 0, 5.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bean with pork soup', 335, 11.5, 7.8, 15.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken broth', 12, 0.5, 0, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef broth powder', 8, 0.3, 0.6, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('split pea soup', 180, 2.3, 12.8, 9.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('escarole soup', 61, 4, 0, 3.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken stock', 86, 2.9, 3.8, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tomato bisque soup', 248, 5, 0, 4.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shark fin soup', 99, 4.3, 0, 6.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('onion soup', 113, 3.5, 6.7, 7.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken gumbo soup', 113, 2.9, 4.9, 5.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crab soup', 95, 0.8, 0, 10.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tripe soup', 148, 5.2, 0, 17.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('little india little lunch', 164, 10.6, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('original chicken sandwich chick fil a', 125, 5.1, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sausage mcmuffin mcdonalds', 383, 24.2, 2.2, 14.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sausage mcgriddles mcdonalds', 421, 24, 15.2, 11.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken drumstick kentucky fried chicken', 70, 3, 0, 10.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken crispy drumsticks kentucky fried chicken', 156, 10.1, 0, 11.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit kentucky fried chicken', 185, 9.1, 1.7, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('english muffin mcdonalds', 97, 2.3, 1.5, 5.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('french fries mcdonalds', 378, 18.1, 0.2, 4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hotdog', 383, 23, 0, 16.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('french fries burger king', 207, 9.2, 0.4, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('original taco with beef cheese lettuce taco bell', 158, 8.8, 0.6, 6.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hotdog with chili', 296, 13.4, 0, 13.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken crispy wings kentucky fried chicken', 148, 10.1, 0, 9.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg white delight mcdonalds', 250, 16, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('french toast sticks burger king', 73, 3.7, 2.1, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mcchicken mcdonalds', 358, 17.3, 4.7, 13.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nachos supreme taco bell', 480, 26.6, 0, 14.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken wing kentucky fried chicken', 104, 5.3, 0, 12.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken strips burger king', 105, 5.5, 0, 6.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken breast kentucky fried chicken', 214, 6.7, 0, 38.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sausage biscuit mcdonalds', 440, 29.7, 2.3, 11.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sausage burrito mcdonalds', 339, 17.1, 1.4, 13) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('onion rings fried', 27, 1.6, 0.4, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('onion rings burger king', 379, 23, 4.9, 3.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turkey patty fried', 79, 5, 0.053, 3.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hotcakes with syrup mcdonalds', 601, 17.8, 45.5, 9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken thigh kentucky fried chicken', 163, 9.1, 0, 20.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn dog', 195, 9.4, 5.9, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken box mcdonalds', 70, 4, 0.5, 0.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('potato wedges kentucky fried chicken', 379, 19.8, 0, 5.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scrambled eggs mcdonalds', 98, 7.5, 0.1, 7.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg mcmuffin mcdonalds', 287, 12.2, 2.7, 17.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nachos taco bell', 362, 21.9, 0, 5.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit large mcdonalds', 310, 14.4, 2.7, 5.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy chicken strips kentucky fried chicken', 129, 7.2, 0, 9.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hotcakes mcdonalds', 113, 2.9, 4.4, 3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken crispy breast kentucky fried chicken', 375, 23.2, 0, 29.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hotcakes sausage mcdonalds', 776, 34.9, 45.7, 15.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken mcnuggets mcdonalds', 48, 3.2, 0.025, 2.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chips chipotle', 151, 7.1, 2.1, 0.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('biscuit mcdonalds', 261, 12.2, 2.2, 4.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice bowl with chicken', 36, 0.4, 1.2, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spaghetti with meatballs', 200, 8.2, 5.8, 8.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('meat ravioli canned', 259, 8.9, 6.3, 8.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese ravioli canned', 186, 3.5, 9, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium crispy chicken ranch blt sandwich mcdonalds', 586, 22.9, 13.3, 35.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('steak sandwich', 459, 14.1, 0, 30.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheeseburger burger king', 380, 19.7, 6, 19.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheeseburger mcdonalds', 313, 14, 7.4, 15.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('double cheeseburger', 643, 36.9, 9.5, 37) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('double hamburger', 942, 58.6, 13.2, 52.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium grilled chicken classic sandwich mcdonalds', 366, 8.6, 10, 28.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('big n tasty with cheese mcdonalds', 573, 36, 9.5, 27.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium grilled chicken club sandwich mcdonalds', 493, 18, 6.4, 38.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('quarter pounder mcdonalds', 417, 19.8, 8.8, 24.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium crispy chicken classic sandwich mcdonalds', 524, 20, 12.4, 27.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('veggie burger', 124, 4.4, 0.7, 11) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('quarter pounder with cheese mcdonalds', 513, 28.3, 9.8, 29) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef sandwich steak raw', 173, 15.1, 0, 9.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheeseburger', 292, 13.2, 5.9, 15) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium grilled chicken ranch blt sandwich mcdonalds', 412, 10.9, 10.5, 33.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('double whopper with cheese burger king', 1061, 68.1, 14.4, 57.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('big mac mcdonalds', 563, 32.8, 8.7, 25.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sandwich with cold cuts', 833, 39.3, 12.4, 41.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('filet o fish mcdonalds', 378, 19.6, 4.9, 15.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hand breaded chicken tenders carls jr', 57, 2.9, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('grilled chicken sandwich wendys', 370, 8, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sandwich with roast beef', 410, 13, 0, 28.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hamburger burger king', 258, 10.4, 5.5, 14.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('big n tasty mcdonalds', 524, 31.7, 8.8, 24.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roast beef sandwich', 364, 15.3, 5.7, 22.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roast beef sandwich with cheese', 473, 18, 0, 32.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hamburger', 255, 9.9, 5.8, 12.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium fish sandwich burger king', 572, 27.4, 7.8, 22.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whopper burger king', 790, 48.4, 13, 35.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sandwich with tuna salad', 584, 28, 0, 29.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('original chicken sandwich burger king', 569, 29.2, 6, 24.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('double whopper burger king', 942, 58.6, 13.2, 52.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('premium crispy chicken club sandwich mcdonalds', 635, 29.9, 12.7, 39.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hamburger mcdonalds', 251, 9.6, 5.7, 12.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pepperoni pizza dominos', 308, 12.6, 4.2, 12.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sausage pizza', 325, 14.3, 3.9, 13.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pepperoni pizza', 313, 13.2, 3.6, 13) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pepperoni pizza pizza hut', 269, 10.9, 3.5, 12.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese pizza pizza hut', 260, 10.5, 3.2, 11.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese pizza', 285, 10.4, 3.8, 12.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese pizza dominos', 278, 9.7, 4.3, 11.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('meat vegetable pizza', 332, 14.8, 5.1, 15) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sucker raw', 146, 3.7, 0, 26.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('northern pike raw', 348, 2.7, 0, 76.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('haddock cooked', 135, 0.8, 0, 30) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burbot cooked', 104, 0.9, 0, 22.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pollock cooked', 356, 3.8, 0, 75.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rainbow smelt raw', 27, 0.7, 0, 5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whitefish cooked', 265, 11.6, 0, 37.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('walleye pike raw', 148, 1.9, 0, 30.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('carp cooked', 275, 12.2, 0, 38.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wolffish cooked', 293, 7.3, 0, 53.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('snapper cooked', 218, 2.9, 0, 44.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roe cooked', 58, 2.3, 0, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('anchovy raw', 37, 1.4, 0, 5.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('herring raw', 291, 16.6, 0, 33) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cod raw', 189, 1.5, 0, 41.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tilapia raw', 111, 2, 0, 23.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whiting cooked', 84, 1.2, 0, 16.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('skipjack tuna cooked', 407, 4, 0, 86.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('salmon cooked', 733, 44, 0, 78.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chum salmon cooked', 474, 14.9, 0, 79.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('american shad cooked', 363, 25.4, 0, 31.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shark cooked', 65, 3.9, 0, 5.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('trout cooked', 118, 5.3, 0, 16.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('swordfish raw', 196, 9, 0, 26.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rainbow trout cooked', 119, 5.2, 0, 16.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish sandwich with cheese', 374, 19.6, 4.9, 15.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('jellyfish dried', 21, 0.8, 0, 3.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mullet raw', 139, 4.5, 0, 23) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('halibut cooked', 353, 5.1, 0, 71.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lingcod raw', 213, 2.7, 0, 44.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('orange roughy cooked', 30, 0.3, 0, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cisco raw', 77, 1.5, 0, 15) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('florida pompano cooked', 186, 10.7, 0, 20.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('snapper raw', 218, 2.9, 0, 44.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish sandwich', 565, 27.4, 7.8, 22.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('haddock raw', 143, 0.9, 0, 31.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ocean perch raw', 51, 1, 0, 9.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cuttlefish cooked', 45, 0.4, 0, 9.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cod cooked', 189, 1.5, 0, 41.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('seatrout raw', 248, 8.6, 0, 39.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('flounder raw', 114, 3.1, 0, 20.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spot cooked', 79, 3.1, 0, 11.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('walleye pike cooked', 148, 1.9, 0, 30.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('perch raw', 55, 0.6, 0, 11.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pumpkin seed sunfish raw', 43, 0.3, 0, 9.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('salt mackerel', 415, 34.1, 0, 25.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mackerel cooked', 231, 15.7, 0, 21) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tilapia cooked', 111, 2.3, 0, 22.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('drum raw', 236, 9.8, 0, 34.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('atlantic croaker fried', 192, 11, 0, 15.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pumpkin seed sunfish cooked', 42, 0.3, 0, 9.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wolffish raw', 294, 7.3, 0, 53.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bluefin tuna cooked', 276, 9.4, 0, 44.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scup cooked', 68, 1.8, 0, 12.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('herring cooked', 290, 16.6, 0, 32.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bluefish raw', 186, 6.4, 0, 30.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sheepshead cooked', 234, 3, 0, 48.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bass cooked', 91, 2.9, 0, 15) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pout cooked', 279, 3.2, 0, 58.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chinook salmon raw', 709, 41.3, 0, 78.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rockfish raw', 172, 2.6, 0, 35.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tilefish raw', 371, 8.9, 0, 67.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scup raw', 174, 4.5, 0, 31.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sablefish cooked', 755, 59.3, 0, 51.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tuna salad', 383, 19, 0, 32.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('coho salmon cooked', 255, 11.8, 0, 34.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roe raw', 20, 0.9, 0, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sea bass raw', 125, 2.6, 0, 23.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pink salmon cooked', 349, 12, 0, 56) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sockeye salmon cooked', 493, 17.3, 0, 82.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whitefish raw', 265, 11.6, 0, 37.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('greenland halibut cooked', 760, 56.4, 0, 58.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('northern pike cooked', 350, 2.7, 0, 76.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('salmon raw', 824, 53.1, 0, 80.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turbot cooked', 388, 12, 0, 65.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('surimi', 28, 0.3, 0, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chinook salmon cooked', 711, 41.2, 0, 79.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mullet cooked', 140, 4.5, 0, 23.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chum salmon raw', 3, 0.1, 0, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('butterfish raw', 47, 2.6, 0, 5.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sea bass cooked', 125, 2.6, 0, 23.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('greenland halibut raw', 759, 56.5, 0, 58.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('halibut raw', 371, 5.4, 0, 75.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sturgeon cooked', 184, 7, 0, 28.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish fillet fried', 211, 11.2, 0, 13.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rainbow trout raw', 111, 4.9, 0, 15.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pout raw', 278, 3.2, 0, 58.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rainbow smelt cooked', 35, 0.9, 0, 6.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('monkfish raw', 22, 0.4, 0, 4.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('trout raw', 117, 5.2, 0, 16.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shark raw', 37, 1.3, 0, 5.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burbot raw', 104, 0.9, 0, 22.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dolphinfish cooked', 173, 1.4, 0, 37.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crayfish cooked', 25, 0.4, 0, 5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dolphinfish raw', 173, 1.4, 0, 37.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('perch cooked', 54, 0.5, 0, 11.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cusk raw', 106, 0.8, 0, 23.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bluefin tuna raw', 41, 1.4, 0, 6.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milkfish cooked', 54, 2.4, 0, 7.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('carp raw', 277, 12.2, 0, 38.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sturgeon raw', 30, 1.1, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('orange roughy raw', 22, 0.2, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('catfish fried', 199, 11.6, 0, 15.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cuttlefish raw', 22, 0.2, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bass raw', 90, 2.9, 0, 14.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('flounder cooked', 109, 3, 0, 19.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milkfish raw', 42, 1.9, 0, 5.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eel raw', 375, 23.8, 0, 37.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('catfish raw', 189, 9.4, 0, 24.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fish sticks', 78, 4.5, 0.5, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('coho salmon raw', 254, 12.2, 0, 33.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('monkfish cooked', 27, 0.6, 0, 5.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellowtail cooked', 546, 19.6, 0, 86.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellowfin tuna cooked', 37, 0.2, 0, 8.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ling cooked', 168, 1.2, 0, 36.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eel cooked', 67, 4.2, 0, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sablefish raw', 753, 59.1, 0, 51.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('salmon nuggets', 60, 3.3, 0, 3.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellowtail raw', 546, 19.6, 0, 86.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('butterfish cooked', 47, 2.6, 0, 5.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caviar', 42, 2.9, 0, 3.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pollock raw', 355, 3.8, 0, 75) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('catfish cooked', 206, 10.3, 0, 26.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whiting raw', 83, 1.2, 0, 16.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pink salmon raw', 404, 14, 0, 65.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellowfin tuna raw', 31, 0.1, 0, 6.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mackerel raw', 230, 15.6, 0, 20.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turbot raw', 388, 12, 0, 65.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cusk cooked', 106, 0.8, 0, 23.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('atlantic croaker raw', 82, 2.5, 0, 14) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sucker cooked', 148, 3.7, 0, 26.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spot raw', 79, 3.1, 0, 11.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('drum cooked', 236, 9.7, 0, 34.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crayfish raw', 3, 0.048, 0, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tilefish cooked', 441, 14.1, 0, 73.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bluefish cooked', 186, 6.4, 0, 30.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('grouper cooked', 238, 2.6, 0, 50.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sheepshead raw', 257, 5.7, 0, 48.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('herring kippered', 87, 4.9, 0, 9.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('grouper raw', 238, 2.6, 0, 50.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ocean perch cooked', 48, 0.9, 0, 9.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('ling raw', 168, 1.2, 0, 36.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lingcod cooked', 329, 4.1, 0, 68.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('florida pompano raw', 184, 10.6, 0, 20.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rockfish cooked', 162, 2.4, 0, 33.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('seatrout cooked', 247, 8.6, 0, 39.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sockeye salmon raw', 562, 22.2, 0, 84.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('swordfish cooked', 182, 8.4, 0, 24.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('american shad raw', 362, 25.3, 0, 31.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sardine canned in oil', 310, 17.1, 0, 36.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('anchovy canned in oil', 8, 0.4, 0, 1.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('salmon canned', 520, 20.3, 0, 79.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sardines in tomato sauce canned', 165, 9.3, 0.4, 18.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sockeye salmon canned', 563, 24.9, 0, 79.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tuna canned', 102, 2.4, 0, 18.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pink salmon canned', 530, 20.4, 0, 80.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cod canned', 328, 2.7, 0, 71) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eastern oyster canned', 110, 4, 0, 11.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chum salmon canned', 520, 20.3, 0, 79.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tuna canned in oil', 331, 14.4, 0, 47.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wild pink salmon oceans', 320, 16, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shrimp canned', 128, 1.7, 0, 26.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('alaska king crab raw', 144, 1, 0, 31.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scallops fried', 64, 3.2, 0, 2.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shrimp cooked', 7, 0.092, 0, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('clams fried', 19, 1, 0, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('alaska king crab cooked', 130, 2.1, 0, 25.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scallops cooked', 11, 0.058, 0, 2.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('squid fried', 140, 6, 0, 14.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oyster raw', 41, 1.2, 0, 4.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('abalone', 89, 0.6, 0, 14.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('queen crab raw', 25, 0.3, 0, 5.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whelk cooked', 78, 0.2, 0, 13.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oyster cooked', 41, 1.2, 0, 4.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eastern oyster cooked', 8, 0.2, 0, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blue mussels cooked', 344, 9, 0, 47.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('octopus raw', 23, 0.3, 0, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spiny lobster cooked', 233, 3.2, 0, 43) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blue mussels raw', 129, 3.4, 0, 17.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('queen crab cooked', 98, 1.3, 0, 20.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dungeness crab cooked', 140, 1.6, 0, 28.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dungeness crab raw', 140, 1.6, 0, 28.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shrimp fried', 52, 3.2, 0.086, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shrimp raw', 4, 0.023, 0, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blue crab cooked', 71, 0.6, 0, 15.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shrimp imitation', 29, 0.4, 0, 3.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('conch baked', 165, 1.5, 0, 33.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('squid raw', 26, 0.4, 0, 4.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scallop raw', 10, 0.091, 0, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lobster raw', 116, 1.1, 0, 24.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('scallop imitation', 28, 0.1, 0, 3.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blue crab raw', 18, 0.2, 0, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whelk raw', 39, 0.1, 0, 6.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spiny lobster raw', 234, 3.2, 0, 43.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('clams canned', 227, 2.5, 0, 38.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lobster cooked', 134, 1.3, 0, 28.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eastern oyster fried', 29, 1.8, 0, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('clams raw', 195, 2.2, 0, 33.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('octopus cooked', 139, 1.8, 0, 25.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eastern oyster raw', 8, 0.2, 0, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oyster fried', 61, 3, 0, 2.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sturgeon smoked', 49, 1.2, 0, 8.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chinook salmon smoked', 159, 5.9, 0, 24.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cisco smoked', 50, 3.4, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('haddock smoked', 33, 0.3, 0, 7.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whitefish smoked', 147, 1.3, 0, 31.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('red salmon sockeye filets smoked', 373, 12.3, 0, 65.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eggnog', 224, 10.6, 20.4, 11.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beer light', 96, 0, 0.3, 0.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beer budweiser', 12, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('weizenbier erdinger', 220, 18, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beer light budweiser', 9, 0, 0, 0.066) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beer', 142, 0, 0, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('root beer', 202, 0, 52.3, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whiskey sour mix', 26, 0.072, 6.3, 0.051) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whiskey sour', 158, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tequila sunrise', 232, 0.2, 0, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('daiquiri', 112, 0.036, 3.3, 0.035) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pina colada', 245, 2.7, 31.5, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whiskey', 58, 0, 0.078, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rum', 49, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vodka smirnoff', 13, 0.4, 0.082, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('coffee liqueur', 96, 0.038, 13.7, 0.041) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gin', 53, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dessert wine sweet', 165, 0, 8, 0.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chenin blanc white wine', 118, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gamay red wine', 115, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('late harvest white wine', 172, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gewurztraminer white wine', 119, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('burgundy red wine', 172, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('zinfandel red wine', 129, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('muller thurgau white wine', 112, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mouvedre red wine', 129, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('table wine', 123, 0, 1.2, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wine light', 73, 0, 1.7, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white wine', 121, 0, 1.4, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('petite sirah red wine', 125, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('non alcoholic wine', 2, 0, 0.3, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pinot noir red wine', 121, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pinot gris grigio white wine', 122, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('riesling white wine', 118, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('barbera red wine', 170, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lemberger red wine', 118, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('merlot red wine', 122, 0, 0.9, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sangiovese red wine', 126, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sauvignon blanc white wine', 119, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('syrah red wine', 122, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('carignane red wine', 370, 0, 0, 0.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pinot blanc white wine', 119, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('semillon white wine', 121, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dessert wine dry', 760, 0, 5.5, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('red wine', 125, 0, 0.9, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cabernet sauvignon red wine', 166, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('muscat white wine', 123, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cooking wine', 2, 0, 0.090, 0.028) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chardonnay white wine', 168, 0, 1.9, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('claret red wine', 122, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice sake', 197, 0, 0, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fume blanc white wine', 121, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cabarnet franc red wine', 122, 0, 0, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate frosting', 81, 3.6, 11.8, 0.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white sugar', 19, 0, 4.9, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg substitute', 115, 0, 4.8, 24) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('agave syrup', 21, 0.081, 4.7, 0.025) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('splenda sweetener', 336, 0, 80.3, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla extract', 12, 0.069, 0.5, 0.000) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg yolk dried', 27, 2.3, 0.002, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('granulated sugar', 16, 0, 4.2, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baking chocolate', 145, 15.2, 0.3, 3.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bakers yeast', 18, 0.3, 0, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fondant', 106, 0.009, 25.2, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brown sugar', 16, 0, 4.1, 0.024) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white frosting dry', 768, 0, 0, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('maple sugar', 42, 0.061, 10.2, 0.036) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fructose sweetener', 15, 0, 3.9, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('saccharin sweetener', 360, 0, 85.2, 0.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg white raw', 17, 0.063, 0.2, 3.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('turbinado sugar', 18, 0, 4.6, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla frosting', 1931, 75, 291.5, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('aspartame sweetener', 365, 0, 80.7, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg raw', 80, 5.3, 0.2, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pineapple topping', 860, 0.3, 71.4, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg white dried', 26, 0.087, 0, 5.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg yolk raw', 55, 4.5, 0.080, 2.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baking soda', 0, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg dried', 30, 2.2, 0.050, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('strawberry topping', 53, 0.096, 5.7, 0.014) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bakers yeast dry', 13, 0.3, 0, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('almond paste', 1040, 63, 82.3, 20.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cream of tartar', 8, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('popover dry mix', 631, 7.3, 0, 17.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg substitute powder', 126, 3.7, 6.2, 15.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gelatin powder', 17, 0.086, 0, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('tapioca pearls', 544, 0.093, 5.1, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baking powder', 2, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg boiled', 93, 6.4, 0.7, 7.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('millet flour', 444, 5.1, 2, 12.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('triticale flour', 439, 2.4, 0, 17.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn flour white', 414, 4.2, 1.8, 9.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice flour brown', 574, 4.4, 1.3, 11.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sorghum flour', 437, 4, 2.3, 9.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cornmeal whole grain', 442, 4.4, 0.8, 9.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cornmeal white', 581, 2.7, 2.5, 11.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('barley flour', 511, 2.4, 1.2, 15.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('safflower seed meal', 97, 0.7, 0, 10.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sesame flour', 658, 46.4, 0, 38.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whole grain wheat flour', 408, 3, 0.5, 15.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn flour yellow', 422, 4.5, 0.7, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn flour whole grain', 422, 4.5, 0.7, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cottonseed flour low fat', 94, 0.4, 0, 14.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sunflower seed flour', 13, 0.013, 0, 1.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sesame meal', 160, 13.6, 0, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('acorn flour', 626, 37.7, 0, 9.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice flour white', 578, 2.2, 0.2, 9.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sesame flour low fat', 94, 0.5, 0, 14.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('potato flour', 571, 0.5, 5.6, 11) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cornmeal yellow', 581, 2.7, 2.5, 11.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rye flour', 364, 1.4, 0.9, 10) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('arrowroot flour', 457, 0.1, 0, 0.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chickpea flour', 356, 6.2, 10, 20.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat flour', 455, 1.2, 0.3, 12.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cornstarch', 488, 0.030, 0, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('carob flour', 13, 0.061, 2.9, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('buckwheat flour', 402, 3.7, 3.1, 15.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pecan pie', 503, 27.1, 0, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apple pie', 331, 15.6, 0, 3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla wafer pie crust', 935, 63.7, 12.9, 6.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate pie crust', 881, 40.8, 47.9, 11.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('apple strudel', 195, 8, 18.3, 2.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate snack cake', 200, 8, 18.9, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit fried pie', 404, 20.6, 27.4, 3.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pumpkin pie', 316, 14.4, 0, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('corn cake', 35, 0.2, 2.1, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sponge snack cake', 157, 4.8, 15.7, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('danish pastry with nuts', 280, 16.4, 16.8, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate cream pie', 301, 19.2, 0, 2.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('angel food cake', 72, 0.2, 0, 1.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('lemon meringue pie', 362, 16.4, 0, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('danish pastry with cinnamon', 349, 16.7, 0, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cinnamon coffeecake', 238, 13.3, 0, 3.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheesecake', 257, 18, 17.4, 4.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruitcake', 139, 3.9, 11.8, 1.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('egg custard', 293, 12.9, 31, 14.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('mince pie', 477, 17.8, 46.7, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellow cake with chocolate frosting', 546, 25.6, 56.5, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('danish pastry with fruit', 335, 15.9, 0, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white cake with coconut frosting', 399, 11.5, 64.3, 4.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('danish pastry with cheese', 353, 24.6, 0, 5.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('graham cracker pie crust', 917, 45.4, 33.2, 9.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blueberry pie', 360, 17.5, 0, 4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellow cake with vanilla frosting', 239, 9.3, 0, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pie crust', 121, 8, 0.045, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate cake with chocolate frosting', 537, 27.7, 55.1, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese coffeecake', 258, 11.6, 0, 5.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla cream pie', 350, 18.1, 16, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pineapple cake', 367, 13.9, 0, 4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peach pie', 262, 11.7, 19, 2.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit coffeecake', 156, 5.1, 0, 2.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pound cake', 116, 6, 0, 1.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('pound cake bread', 215, 9.6, 10, 3.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white cake', 264, 9.2, 26.3, 4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('coffeecake with chocolate frosting', 298, 9.7, 0, 4.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('popcorn cake', 38, 0.3, 0.001, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('banana cream pie', 3190, 161.3, 143, 52.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('baked apple pie mcdonalds', 91, 4.4, 4.9, 0.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sponge cake', 187, 2.7, 0, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('yellow cake', 245, 9.9, 0, 3.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('shortcake', 98, 4, 0, 1.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cherry pie', 486, 22, 0, 5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dutch apple pie', 397, 15.8, 30.2, 3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate cake', 352, 14.3, 0, 5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('marshmallow', 64, 0.033, 11.5, 0.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sugar apple', 72, 0.2, 0, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('toffee', 67, 3.9, 7.6, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla fudge with nuts', 123, 3.9, 20.1, 0.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanuts chocolate coated', 21, 1.3, 1.5, 0.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caramel with nuts chocolate coated', 66, 2.9, 5.8, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gingerbread', 263, 12.1, 0, 2.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rolo nestle', 24, 1, 0.078, 0.009) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla fudge', 108, 1.5, 22.6, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('halavah', 328, 15.1, 0, 8.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('praline', 189, 10.1, 21.8, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit juice bar', 67, 0.089, 13.5, 0.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut brittle', 138, 5.4, 14.5, 2.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('butterscotch', 21, 0.2, 4.3, 0.079) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate coffee beans', 155, 9.4, 14.4, 2.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('nougat with almonds', 56, 0.2, 11.7, 0.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('carob', 135, 7.8, 8.5, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut butter fudge', 62, 1.1, 11.7, 0.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sugar coated almonds', 17, 0.6, 2.2, 0.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate coated marshmallow', 118, 4.7, 12.5, 1.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('raisins chocolate coated', 390, 14.8, 62.2, 4.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chewing gum sugarless', 5, 0.013, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chewing gum', 11, 0.032, 2, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate syrup', 109, 0.4, 19.4, 0.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate fudge syrup', 67, 1.7, 6.6, 0.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('m ms milk chocolate mars', 207, 8.9, 26.7, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milk chocolate with almonds', 216, 14.1, 18, 3.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate fudge', 70, 1.8, 12.4, 0.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milk chocolate with rice cereal', 0, 0, 0, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('m ms peanut chocolate mars', 93, 4.7, 9.1, 1.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dark chocolate', 165, 9.7, 13.8, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate semisweet', 806, 50.4, 91.6, 7.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('after eight mints nestle', 36, 1, 5.6, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milk chocolate', 37, 2.1, 3.6, 0.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white chocolate', 162, 9.6, 17.7, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate', 208, 14, 21.1, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('twix mars', 146, 7.2, 14, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('kit kat nestle', 62, 3.3, 6.5, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('snickers mars', 280, 13.6, 28.8, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milky way mars', 264, 10, 34.6, 2.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('clif bar', 235, 4, 21.5, 10) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('gum drops', 18, 0.042, 3.5, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('skittles wild berry mars', 249, 2.6, 47.1, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('skittles original mars', 251, 2.7, 47, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('skittles tropical mars', 251, 2.7, 47, 0.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit leather', 52, 0.4, 6.9, 0.029) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('taffy', 60, 0.5, 10.3, 0.087) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('eisbonbons mac iver', 23, 0, 0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caramels', 39, 0.8, 6.6, 0.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('hard candy', 24, 0.067, 3.8, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('jellybeans', 4, 0.043, 0.8, 0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('jellies', 56, 0.069, 10.8, 0.005) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat germ toasted', 115, 3.2, 2.3, 8.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crispy brown rice', 124, 1.1, 2.9, 2.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bran flakes asda', 109, 0.6, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('amaranth cooked', 251, 3.9, 0, 9.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('triticale', 645, 4, 0, 25.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('barley raw', 42, 0.1, 0.094, 1.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bulgur dry', 171, 0.7, 0.2, 6.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cottonseed kernels roasted', 754, 54.1, 0, 48.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice bran', 373, 24.6, 1.1, 15.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chaptti roti indian bread', 491, 2.5, 68.4, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('quinoa cooked', 222, 3.6, 1.6, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('dar vida swiss original', 116, 3.1, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('teff cooked', 255, 1.6, 0, 9.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('couscous dry', 650, 1.1, 0, 22.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('weetabix weetabix', 2078, 11.5, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oat bran cooked', 88, 1.9, 0, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('buckwheat cooked', 155, 1, 1.5, 5.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('millet puffed', 74, 0.7, 0.1, 2.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oat bran raw', 23, 0.5, 0.1, 1.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('popcorn oil popped', 64, 4.8, 0.027, 0.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('kamut cooked', 251, 1.6, 0, 11.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat bran', 125, 2.5, 0.2, 9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese popcorn', 58, 3.7, 0, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('raisin bran crunch kelloggs', 90, 0.5, 9, 1.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('couscous cooked', 176, 0.3, 0.2, 6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('multigrain cheerios general mills', 4, 0.005, 0.2, 0.038) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('bulgur cooked', 151, 0.4, 0.2, 5.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('popcorn air popped', 31, 0.4, 0.086, 1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('teff raw', 708, 4.6, 3.6, 25.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spelt cooked', 246, 1.6, 0, 10.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('millet raw', 756, 8.4, 0, 22) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('popcorn unpopped', 106, 1.2, 0.3, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whole wheat cooked', 150, 1, 0.2, 4.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat sprouted', 214, 1.4, 0, 8.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('whole wheat dry', 96, 0.6, 0.1, 3.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('caramel popcorn', 122, 3.6, 15.1, 1.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat puffed', 44, 0.1, 0, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('roggenmischbrot sonnenblumenkerne aldi', 82, 3.1, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat germ', 112, 3, 0, 7.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('microwave popcorn', 465, 26.3, 0.3, 7.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('millet cooked', 207, 1.7, 0.2, 6.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('kamut raw', 627, 4.1, 15.2, 27.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat durum', 651, 4.7, 0, 26.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('sorghum', 651, 6.3, 0, 21.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white wheat', 657, 3.3, 0.8, 21.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rye', 571, 2.8, 1.7, 17.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('spelt raw', 588, 4.2, 11.9, 25.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('buckwheat raw', 41, 0.4, 0, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('honey cereali general mills', 104, 2, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('barley cooked', 193, 0.7, 0.4, 3.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('oats', 39, 0.7, 0, 1.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('amaranth raw', 37, 0.7, 0.2, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('semolina', 601, 1.8, 0, 21.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('quinoa dry', 221, 3.6, 0, 8.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wheat shredded', 155, 1, 0.4, 5.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cattail', 5, 0, 0.074, 0.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut granola bar', 116, 5.7, 8.2, 2.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('vanilla bluberry bar kind', 49, 1.4, 0.096, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('coconut granola bar', 150, 9.1, 9.7, 1.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('almond granola bar', 119, 6.1, 0, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chocolate chip granola bar', 105, 3.9, 0, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit granola bar', 82, 0.2, 13.3, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('granola bar', 99, 4.2, 6, 2.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('peanut bar', 209, 13.5, 16.9, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('milk cereal bar', 103, 2.7, 11.5, 1.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('almond rice bar', 128, 5.7, 0, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fruit nut squares', 55, 1.3, 6.4, 0.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice wheat cereal bar', 90, 2, 7, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('crisped rice bar', 113, 3.8, 0, 1.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brown rice raw', 685, 5.4, 1.6, 14.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice pilaf cooked', 352, 8.8, 0.6, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef flavored rice raw', 657, 2.3, 0, 19.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wild rice cooked', 166, 0.6, 1.2, 6.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('wild rice raw', 571, 1.7, 4, 23.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('congee with pork shrimp and squid plus egg', 178, 0.3, 0.0, 0.0) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white rice pasta cooked', 246, 5.7, 0, 5.1) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken flavored rice raw', 601, 2.2, 4, 17.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('instant white rice raw', 361, 0.9, 0.026, 7.4) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice crisps', 107, 0.4, 2.3, 1.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('parboiled white rice raw', 692, 1.9, 0.6, 13.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('instant white rice cooked', 193, 0.8, 0, 3.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice pilaf raw', 732, 2.8, 3.1, 21.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('rice cracker cake', 111, 1.2, 0.2, 2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('parboiled white rice cooked', 194, 0.6, 0.2, 4.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white rice steamed', 199, 0.4, 0, 4.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brown rice cake', 35, 0.3, 0.047, 0.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('glutinous white rice raw', 685, 1, 0, 12.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white rice pasta raw', 600, 4, 0, 15.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('glutinous white rice cooked', 169, 0.3, 0.078, 3.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('brown rice cooked', 218, 1.6, 0.7, 4.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('beef flavored rice cooked', 319, 7.9, 0, 7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white rice cooked', 205, 0.4, 0.015, 4.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('chicken flavored rice cooked', 317, 8, 1.5, 6.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('white rice raw', 675, 1.2, 0.2, 13.2) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheese imitation', 441, 36.2, 1.1, 28.3) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('cheshire cheese', 110, 8.7, 0, 6.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('monterey cheese', 421, 34.2, 0.6, 27.7) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('edam cheese', 107, 8.3, 0, 7.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('fontina cheese', 420, 33.6, 1.7, 27.6) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('monterey cheese low fat', 350, 24.4, 0.6, 31.9) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('colby cheese', 445, 36.3, 0.6, 26.8) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('american cheese', 93, 7.9, 0.6, 4.5) 
GO 
insert into FoodNutritions(Name, CaloricValue, Fat, Sugars, Protein) Values ('blue cheese', 106, 8.6, 0.2, 6.4) 
GO 
